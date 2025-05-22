using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Verx.Consolidated.Common.Contracts;

namespace Verx.Consolidated.Common.Tracing;

/// <summary>
/// Represents a wrapper around the <see cref="System.Diagnostics.Activity"/> class, providing a custom implementation
/// of the <see cref="IActivity"/> interface for tracing and telemetry within the Verx Transaction Flow system.
/// </summary>
/// <remarks>
/// <para>
/// This class is designed to encapsulate the lifecycle and tagging/status operations of an <see cref="Activity"/> instance.
/// It ensures proper disposal and provides a fluent interface for setting tags and status codes.
/// </para>
/// <para>
/// Usage:
/// <code>
/// using var verxActivity = new VerxActivity(activity);
/// verxActivity.SetTag("key", "value").SetStatus(ActivityStatusCode.Ok);
/// </code>
/// </para>
/// </remarks>
/// <example>
/// <code>
/// using (var verxActivity = new VerxActivity(activity))
/// {
///     verxActivity.SetTag("operation", "payment")
///                 .SetStatus(ActivityStatusCode.Ok, "Operation completed successfully");
/// }
/// </code>
/// </example>
public class VerxActivity(ILogger logger, Activity activity) : IActivity
{
    /// <summary>
    /// Disposes the underlying <see cref="Activity"/> by stopping and releasing its resources.
    /// </summary>
    public void Dispose()
    {
        activity.Stop();
        activity.Dispose();
        activity = null;
    }

    /// <inheritdoc/>
    public IActivity SetTag(string key, object value)
    {
        activity.SetTag(key, value);
        return this;
    }

    /// <inheritdoc/>
    public IActivity SetStatus(ActivityStatusCode activityStatusCode, string? description = null)
    {
        activity.SetStatus(activityStatusCode, description);
        return this;
    }

    /// <inheritdoc/>
    public IActivity LogMessage(string message)
    {
        activity.SetTag("message", message);
        logger.LogInformation(message);

        return this;
    }

    /// <inheritdoc/>
    public void Success()
    {
        activity.SetStatus(ActivityStatusCode.Ok);
    }

    /// <inheritdoc/>
    public void Failure(string? description = null)
    {
        activity.SetStatus(ActivityStatusCode.Error, description);
    }
}
