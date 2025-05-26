using System.Diagnostics;

namespace Verx.Enterprise.Tracing;

/// <summary>
/// Represents a tracing span, which is a logical unit of work within a distributed tracing system.
/// </summary>
public interface ISpan : IDisposable
{
    /// <summary>
    /// Gets the underlying <see cref="ActivityContext"/> associated with this span.
    /// </summary>
    ActivityContext Context { get; }

    /// <summary>
    /// Sets a key-value pair as metadata on the span.
    /// </summary>
    /// <param name="key">The key for the metadata entry.</param>
    /// <param name="value">The value associated with the key.</param>
    /// <returns>The current <see cref="ISpan"/> instance for chaining.</returns>
    ISpan SetKey(string key, object value);

    /// <summary>
    /// Adds a new message to the span, which can be used for logging or annotating the span with additional information.
    /// </summary>
    /// <param name="message">The message to add to the span.</param>
    /// <returns>The current <see cref="ISpan"/> instance for chaining.</returns>
    ISpan NewMessage(string message);

    /// <summary>
    /// Marks the span as successfully completed.
    /// </summary>
    void Success();

    /// <summary>
    /// Marks the span as failed, optionally providing a description of the failure.
    /// </summary>
    /// <param name="description">An optional description of the failure.</param>
    void Failure(string? description = null);
}
