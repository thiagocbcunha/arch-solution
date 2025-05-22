using Confluent.Kafka;
using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Verx.TransactionFlow.Domain.Options;
using Verx.TransactionFlow.Domain.Contracts;
using Verx.TransactionFlow.Common.Contracts;
using Verx.TransactionFlow.Infrastructure.MessageBrokers.RabbitMQ;

namespace Verx.TransactionFlow.Infrastructure.MessageBrokers.Kafka;

/// <summary>
/// KafkaConsumer is a background service responsible for consuming messages from a Kafka topic,
/// processing them, and forwarding them to a channel for further handling. It also integrates
/// distributed tracing and error handling by forwarding failed messages to a RabbitMQ producer.
/// </summary>
/// <typeparam name="TMessage">
/// The type of the message payload being consumed. Must be a reference type.
/// </typeparam>
/// <remarks>
/// This consumer subscribes to a Kafka topic named after the message type (e.g., "Order_topic"),
/// deserializes incoming messages into <see cref="EventMessage{TMessage}"/>, and writes them to
/// a provided channel. It uses activity tracing for observability and logs processing details.
/// If a message is not acknowledged as successful, it is forwarded to a RabbitMQ producer for
/// further handling or retry.
/// </remarks>
/// <param name="logger">Logger for diagnostic and operational messages.</param>
/// <param name="kafkaSettings">Kafka configuration options, including topic, bootstrap servers, and consumer group.</param>
/// <param name="activityTracing">Tracing provider for distributed tracing and activity logging.</param>
/// <param name="rabbitProducer">RabbitMQ producer for forwarding failed messages.</param>
/// <param name="channel">Channel for writing successfully consumed event messages.</param>
/// <param name="messageCorrelation">Correlation context for associating messages with a correlation ID.</param>
public class KafkaConsumer<TMessage>
    (IRabbitProducer rabbitProducer,
    IActivityTracing activityTracing,
    IOptions<KafkaSettings> kafkaSettings, 
    Channel<EventMessage<TMessage>> channel, 
    ILogger<KafkaConsumer<TMessage>> logger, 
    EventMessageCorrelation messageCorrelation) : BackgroundService
        where TMessage : class
{
    private readonly KafkaSettings _settings = kafkaSettings.Value;
    private readonly IConsumer<Ignore, EventMessage<TMessage>> _consumer = new ConsumerBuilder<Ignore, EventMessage<TMessage>>(new ConsumerConfig
    {
        EnableAutoCommit = true,
        AutoOffsetReset = AutoOffsetReset.Earliest,
        GroupId = kafkaSettings.Value.ConsumerGroup,
        BootstrapServers = kafkaSettings.Value.BootstrapServers,
    })
        .SetValueDeserializer(new EventMessageDeserializer<TMessage>())
        .Build();

    /// <summary>
    /// Executes the background service, repeatedly attempting to start consuming messages.
    /// If consumption fails, the service waits and retries until cancellation is requested.
    /// </summary>
    /// <param name="stoppingToken">Token to signal cancellation of the background service.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!await StartConsuming(stoppingToken))
        {
            logger.LogInformation("Kafka consumer service is restarting in 5 seconds.");
            await Task.Delay(5000, stoppingToken);
        }

        _consumer.Close();
        _consumer.Dispose();

        await Task.FromCanceled(stoppingToken);
    }

    /// <summary>
    /// Starts consuming messages from the configured Kafka topic. For each message:
    /// <list type="bullet">
    /// <item>Begins a tracing activity for observability.</item>
    /// <item>Sets the correlation ID for the message context.</item>
    /// <item>Writes the message to the provided channel.</item>
    /// <item>If the message is not acknowledged as successful, logs the error and forwards the message to RabbitMQ.</item>
    /// <item>Commits the message offset in Kafka.</item>
    /// </list>
    /// If an exception occurs, logs the service stop and returns false to trigger a retry.
    /// </summary>
    /// <param name="stoppingToken">Token to signal cancellation of the consuming loop.</param>
    /// <returns>True if consumption completes successfully; otherwise, false to indicate a retry is needed.</returns>
    protected async Task<bool> StartConsuming(CancellationToken stoppingToken)
    {
        logger.LogInformation($"Consuming messages from topic: {_settings.Topic}");

        try
        {
            _consumer.Subscribe($"{typeof(TMessage).Name}_topic");

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(stoppingToken);

                using var activity = activityTracing.Start<KafkaConsumer<TMessage>>(ActivityKind.Consumer);
                messageCorrelation.SetCorrelation(consumeResult.Message.Value.CorrelationId);

                activity.LogMessage($"Received event: {consumeResult.Message.Value}");

                channel.Writer.WriteAsync(consumeResult.Message.Value, stoppingToken).AsTask().Wait(stoppingToken);

                if (!consumeResult.Message.Value.Success)
                {
                    activity.LogMessage($"Error processing message: {consumeResult.Message.Value}");
                    activity.Failure("Message not acknowledged.");
                    await rabbitProducer.SendMessageAsync(consumeResult.Message.Value.Message, stoppingToken);
                }
                else
                    activity.Success();

                _consumer.Commit(consumeResult);
            }

            return true;
        }
        catch { }

        logger.LogInformation("Kafka consumer background service has stopped.");

        return false;
    }
}
