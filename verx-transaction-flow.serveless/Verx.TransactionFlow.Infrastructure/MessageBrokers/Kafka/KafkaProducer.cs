using Confluent.Kafka;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Verx.TransactionFlow.Domain.Options;
using Verx.TransactionFlow.Common.Logging;
using Verx.TransactionFlow.Domain.Contracts;
using Verx.TransactionFlow.Common.Contracts;
using Confluent.Kafka.Extensions.Diagnostics;

namespace Verx.TransactionFlow.Infrastructure.MessageBrokers.Kafka;

/// <summary>
/// KafkaProducer is responsible for producing messages to a Kafka topic using the Confluent.Kafka library.
/// It serializes messages, adds correlation and telemetry information, and sends them to the configured Kafka topic.
/// </summary>
/// <remarks>
/// This class uses dependency injection for logging, telemetry, configuration, and correlation.
/// It implements the <see cref="IKafkaProducer"/> interface and extends <see cref="BaseProducer"/> for message creation and serialization.
/// </remarks>
public class KafkaProducer<TMessage>(ILogger<KafkaProducer<TMessage>> logger, IActivityTracing activityFactory, IOptions<KafkaSettings> options, ICorrelation correlation) : BaseProducer(correlation), IKafkaProducer<TMessage>
    where TMessage : class
{
    private readonly IProducer<Null, EventMessage<TMessage>> _producer =
        new ProducerBuilder<Null, EventMessage<TMessage>>(new ProducerConfig { BootstrapServers = options.Value.BootstrapServers })
        .SetValueSerializer(new EventMessageSerializer<TMessage>())
        .BuildWithInstrumentation();

    // <inheritdoc />
    public async Task SendMessageAsync(TMessage message, CancellationToken cancellationToken)
    {
        using var activity = activityFactory.Create<KafkaProducer<TMessage>>(ActivityKind.Producer);
        activity.LogMessage("Sending Kafka Message");

        var kafkaMessage = new Message<Null, EventMessage<TMessage>>
        {
            Value = CreateEventMessage(message)
        };

        await _producer.ProduceAsync($"{typeof(TMessage).Name}_topic", kafkaMessage, cancellationToken);

        activity.Success();
    }
}
