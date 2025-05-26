using Serilog.Context;
using Confluent.Kafka;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Verx.Enterprise.MessageBroker.RabbitMQ.Extensions;

namespace Verx.Enterprise.MessageBroker.Kafka.Client;

[ExcludeFromCodeCoverage]
public class KafkaConsumer<TMessage>(ILogger<KafkaConsumer<TMessage>> logger, Channel<EventPackage<TMessage>> channel, ConsumerConfig config, IServiceProvider provider) : BackgroundService
    where TMessage : class, new()
{
    private readonly string _topic = $"{typeof(TMessage).Name.ToLower()}_topic";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!await StartConsuming(stoppingToken))
        {
            logger.LogInformation("KafkaConsumer is recycling.");

            await Task.Delay(5000, stoppingToken);
        }
    }

    private Task<bool> StartConsuming(CancellationToken stoppingToken)
    {
        try
        {
            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(_topic);
            logger.LogInformation("KafkaConsumer is running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var eventMessage = consumer.Consume(stoppingToken);
                var correlation = eventMessage.Message.Headers.CorrelationId();
                var parentContext = eventMessage.Message.Headers.ExtractContext();

                using (var outerScope = provider.CreateScope())
                {
                    var tracer = outerScope.ServiceProvider.GetRequiredService<InternalTraceFacade>();
                    using (LogContext.PushProperty("CorrelationId", correlation))
                    {
                        using var span = tracer.CreateConsumerSpan<KafkaConsumer<TMessage>, TMessage>(parentContext, correlation, "kafka");
                        ValueTask valueTask = channel.Writer.WriteAsync(new EventPackage<TMessage> { KafkaMessage = eventMessage, ParentContext = parentContext }, stoppingToken);

                        if (valueTask.IsCompleted)
                            span.Success();

                        consumer.Commit(eventMessage);
                    }
                }
            }

            consumer.Close();
        }
        catch
        {
        }

        return Task.FromResult(false);
    }
}
