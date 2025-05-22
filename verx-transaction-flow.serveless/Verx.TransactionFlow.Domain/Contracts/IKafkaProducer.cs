namespace Verx.TransactionFlow.Domain.Contracts;

/// <summary>
/// Interface for a Kafka producer.
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public interface IKafkaProducer<TMessage>
    where TMessage : class
{
    /// <summary>
    /// Sends a message to the Kafka topic.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendMessageAsync(TMessage message, CancellationToken cancellationToken);
}