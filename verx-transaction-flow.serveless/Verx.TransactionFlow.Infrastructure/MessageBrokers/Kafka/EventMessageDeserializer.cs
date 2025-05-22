using Confluent.Kafka;
using System.Text.Json;

namespace Verx.TransactionFlow.Infrastructure.MessageBrokers.Kafka;
public class EventMessageDeserializer<TMessage> : IDeserializer<EventMessage<TMessage>>
    where TMessage : class
{
    public EventMessage<TMessage> Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull || data.IsEmpty)
            return null!;

        return JsonSerializer.Deserialize<EventMessage<TMessage>>(data)!;
    }
}
