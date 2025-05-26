namespace Verx.Enterprise.MessageBroker.Kafka;

public interface IKafkaProducer<TMessage> : IProducer<TMessage>
    where TMessage : class
{
}