using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Verx.Enterprise.Tracing;

/// <summary>
/// Represents a tracing span that wraps an <see cref="Activity"/> instance.
/// Provides methods to set status, add metadata, and mark the span as successful or failed.
/// Implements <see cref="ISpan"/> and <see cref="IDisposable"/> for integration with distributed tracing systems.
/// </summary>
[ExcludeFromCodeCoverage]
public class Span(Activity activity) : ISpan, IDisposable
{
    /// <inheritdoc/>
    public ActivityContext Context => activity.Context;

    /// <summary>
    /// Sets the status of the underlying <see cref="Activity"/> to the specified <paramref name="activityStatusCode"/>,
    /// optionally providing a description.
    /// </summary>
    /// <param name="activityStatusCode">The status code to set on the activity.</param>
    /// <param name="description">An optional description providing additional context for the status.</param>
    /// <returns>The current <see cref="ISpan"/> instance for method chaining.</returns>
    private ISpan SetStatus(ActivityStatusCode activityStatusCode, string? description = null)
    {
        activity.SetStatus(activityStatusCode, description);
        return this;
    }

    /// <inheritdoc/>
    public ISpan SetKey(string key, object value)
    {
        activity.SetTag(key, value);
        return this;
    }

    /// <inheritdoc/>
    public ISpan NewMessage(string message)
    {
        SetKey("message", message);
        return this;
    }

    /// <inheritdoc/>
    public void Success()
    {
        SetStatus(ActivityStatusCode.Ok);
    }

    /// <inheritdoc/>
    public void Failure(string? description = null)
    {
        SetStatus(ActivityStatusCode.Error, description);
    }

    /// <summary>
    /// Disposes the underlying <see cref="Activity"/> by stopping and releasing its resources.
    /// </summary>
    public void Dispose()
    {
        activity.Stop();
        activity.Dispose();
    }
}
