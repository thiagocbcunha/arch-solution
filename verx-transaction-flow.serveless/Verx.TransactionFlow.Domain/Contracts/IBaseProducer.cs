namespace Verx.TransactionFlow.Domain.Contracts;

/// <summary>
/// Interface for a producer.
/// </summary>
public interface IBaseProducer
{
    /// <summary>
    /// Sends a message to the Kafka topic.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendMessageAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
        where TMessage : class;
}
