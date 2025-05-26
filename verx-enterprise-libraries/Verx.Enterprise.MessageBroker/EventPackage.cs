using Confluent.Kafka;
using RabbitMQ.Client.Events;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics.CodeAnalysis;

namespace Verx.Enterprise.MessageBroker;

[ExcludeFromCodeCoverage]
public class EventPackage<TMessage>
{
    public PropagationContext ParentContext { get; internal set; }

    internal bool IsKafka => KafkaMessage is not null;
    internal bool IsRabbit => RabbitMessage is not null;
    internal BasicDeliverEventArgs? RabbitMessage { get; init; }
    internal ConsumeResult<string, string>? KafkaMessage { get; init; }
}
