using Confluent.Kafka;
using System.Text.Json;

namespace Verx.TransactionFlow.Infrastructure.MessageBrokers.Kafka;

public class EventMessageSerializer<TMessage> : ISerializer<EventMessage<TMessage>>
    where TMessage : class
{
    public byte[] Serialize(EventMessage<TMessage> data, SerializationContext context)
    {
        if (data == null)
            return [];

        return JsonSerializer.SerializeToUtf8Bytes(data);
    }
}
