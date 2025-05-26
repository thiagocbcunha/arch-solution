using System.Text;
using OpenTelemetry;
using Confluent.Kafka;
using System.Text.Json;
using Verx.Enterprise.Correlation;
using System.Diagnostics.CodeAnalysis;
using OpenTelemetry.Context.Propagation;
using Headers = Confluent.Kafka.Headers;

namespace Verx.Enterprise.MessageBroker.Kafka.Client;

[ExcludeFromCodeCoverage]
public class KafkaProducer<TMessage>(ProducerConfig config, InternalTraceFacade tracer, ICorrelation correlation) : IKafkaProducer<TMessage>
    where TMessage : class
{
    private readonly string _topic = $"{typeof(TMessage).Name.ToLower()}_topic";

    private readonly IProducer<string, string> _producer = new ProducerBuilder<string, string>(config).Build();

    public async Task PublishAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        using var activity = tracer.CreateProducerSpan<KafkaProducer<TMessage>, TMessage>("kafka");
        var json = JsonSerializer.Serialize(message);

        var kafkaMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = json,
            Headers = []
        };

        Propagators.DefaultTextMapPropagator.Inject(new PropagationContext(activity.Context, Baggage.Current), kafkaMessage.Headers, InjectHeader);

        kafkaMessage.Headers.Add(Constants.CorrelationIdHeader, Encoding.UTF8.GetBytes(correlation.Id.ToString()));

        await _producer.ProduceAsync(_topic, kafkaMessage, cancellationToken);
    }

    private static void InjectHeader(Headers headers, string key, string value)
    {
        headers.Add(key, Encoding.UTF8.GetBytes(value));
    }
}
