using System.Text.Json;
using Verx.TransactionFlow.Common.Logging;

namespace Verx.TransactionFlow.Infrastructure.MessageBrokers;

public abstract class BaseProducer(ICorrelation correlation)
{
    /// <summary>
    /// Creates an event message with the specified message and event name.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="message"></param>
    /// <param name="eventName"></param>
    /// <returns></returns>
    internal EventMessage<TMessage> CreateEventMessage<TMessage>(TMessage message)
        where TMessage : class
        => new()
        {
            CorrelationId = correlation.Id,
            EventName = typeof(TMessage).Name,
            Message = message
        };

    /// <summary>
    /// Creates an event name for the specified message and event name.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="message"></param>
    /// <param name="eventName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal Task<string> CreateEventMessage<TMessage>(TMessage message, CancellationToken cancellationToken)
        where TMessage : class
    {
        var eventMessage = CreateEventMessage(message);
        return SerializeMessage(eventMessage, cancellationToken);
    }

    /// <summary>
    /// Serializes the message to a JSON string.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="eventMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal async Task<string> SerializeMessage<TMessage>(EventMessage<TMessage> eventMessage, CancellationToken cancellationToken)
        where TMessage : class
    {
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, eventMessage, cancellationToken: cancellationToken);
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var serializedMessage = await reader.ReadToEndAsync(cancellationToken);

        return serializedMessage;
    }
}
