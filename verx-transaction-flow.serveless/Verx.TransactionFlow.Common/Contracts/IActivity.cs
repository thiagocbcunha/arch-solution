using System.Diagnostics;

namespace Verx.TransactionFlow.Common.Contracts;

/// <summary>
/// Represents an activity within a transaction flow, providing methods to track its status, set tags, and manage its lifecycle.
/// </summary>
/// <remarks>
/// Implementations of this interface are expected to encapsulate the logic for marking an activity as successful or failed,
/// attaching metadata via tags, and setting the activity's status code and description. The interface also inherits from <see cref="IDisposable"/>,
/// indicating that resources associated with the activity should be released when it is no longer needed.
/// </remarks>
public interface IActivity : IDisposable
{
    /// <summary>
    /// Marks the activity as successful.
    /// </summary>
    void Success();

    /// <summary>
    /// Marks the activity as failed, optionally providing a description of the failure.
    /// </summary>
    /// <param name="description">An optional description explaining the reason for failure.</param>
    void Failure(string? description = null);

    /// <summary>
    /// Logs a message to the activity, which can be used for debugging or informational purposes.
    /// </summary>
    /// <param name="message">Message for tag message.</param>
    /// <returns></returns>
    IActivity LogMessage(string message);

    /// <summary>
    /// Attaches a tag to the activity for additional metadata.
    /// </summary>
    /// <param name="name">The name of the tag.</param>
    /// <param name="value">The value of the tag.</param>
    /// <returns>The current <see cref="IActivity"/> instance for method chaining.</returns>
    IActivity SetTag(string name, object value);

    /// <summary>
    /// Sets the status code and optional description for the activity.
    /// </summary>
    /// <param name="activityStatusCode">The status code representing the activity's outcome.</param>
    /// <param name="description">An optional description providing additional context for the status.</param>
    /// <returns>The current <see cref="IActivity"/> instance for method chaining.</returns>
    IActivity SetStatus(ActivityStatusCode activityStatusCode, string? description = null);
}
