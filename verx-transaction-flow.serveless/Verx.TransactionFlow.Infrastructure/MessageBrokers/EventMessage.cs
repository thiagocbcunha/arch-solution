using System.Text.Json.Serialization;

namespace Verx.TransactionFlow.Infrastructure.MessageBrokers;

/// <summary>
/// Represents a generic event message used for message brokering.
/// Contains metadata such as correlation ID, event name, and the message payload.
/// Provides an acknowledgment mechanism to mark the message as successfully processed.
/// </summary>
/// <typeparam name="TMessage">
/// The type of the message payload. Must be a reference type.
/// </typeparam>
public class EventMessage<TMessage>
    where TMessage : class
{
    /// <summary>
    /// Gets or sets the unique identifier used to correlate this event message with related operations.
    /// </summary>
    [JsonPropertyName("correlationId")]
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the name of the event.
    /// </summary>
    [JsonPropertyName("eventName")]
    public required string EventName { get; set; }

    /// <summary>
    /// Gets or sets the message payload.
    /// </summary>
    [JsonPropertyName("message")]
    public required TMessage Message { get; set; }

    /// <summary>
    /// Gets a value indicating whether the message has been acknowledged as successfully processed.
    /// </summary>
    [JsonIgnore]
    public bool Success { get; private set; } = false;

    /// <summary>
    /// Marks the message as successfully processed.
    /// </summary>
    public void Ack() => Success = true;
}
