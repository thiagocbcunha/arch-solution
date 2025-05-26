namespace Verx.Enterprise.MessageBroker;

public interface IProducer<in TMessage>
    where TMessage : class
{
    Task PublishAsync(TMessage message, CancellationToken cancellationToken = default);
}
