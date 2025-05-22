using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Verx.TransactionFlow.Common.Options;
using Verx.TransactionFlow.Common.Logging;
using Verx.TransactionFlow.Common.Contracts;

namespace Verx.TransactionFlow.Common.Tracing;

/// <summary>
/// Provides an implementation of <see cref="IActivityTracing"/> for managing and tracing activities
/// within the Verx Transaction Flow system. This class is responsible for starting, tagging, logging,
/// and setting the status of activities, as well as handling their lifecycle and correlation context.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="VerxActivityTracing"/> wraps the creation and management of <see cref="Activity"/> instances
/// using the provided <see cref="ObservabilitySettings"/> and <see cref="ICorrelation"/> for context propagation.
/// It ensures that each activity is properly started, tagged with a correlation ID, and disposed of when no longer needed.
/// </para>
/// <para>
/// Usage:
/// <code>
/// var tracing = new VerxActivityTracing(settings, correlation);
/// using var activity = tracing.Start&lt;MyService&gt;();
/// activity.SetTag("key", "value").SetStatus(ActivityStatusCode.Ok);
/// </code>
/// </para>
/// </remarks>
public class VerxActivityTracing(ILoggerFactory loggerFactory, ObservabilitySettings config, ICorrelation correlation) : IActivityTracing
{
    private Activity? _currentActivity;
    private readonly ActivitySource _mainSource = new(config.Name, config.Version);

    /// <inheritdoc/>
    public IActivity Start<TCaller>(ActivityKind activityKind)
    {
        var name = GetName(typeof(TCaller));
        var logger = loggerFactory.CreateLogger(GetType());
        _currentActivity = _mainSource.StartActivity(name, activityKind);
        _currentActivity?.Start();

        ArgumentNullException.ThrowIfNull(_currentActivity);

        var verxActivity = new VerxActivity(logger, _currentActivity);
        _currentActivity.SetTag("CorrelationId", correlation.Id);
        
        return verxActivity;
    }

    /// <inheritdoc/>
    public IActivity Create(string identify, ActivityKind activityKind = ActivityKind.Internal)
    {
        var logger = loggerFactory.CreateLogger(GetType());
        return Create(logger, identify, activityKind);
    }

    /// <inheritdoc/>
    public IActivity Create<TCaller>(ActivityKind activityKind = ActivityKind.Internal)
    {
        var type = typeof(TCaller);
        string name = GetName(type);

        var logger = loggerFactory.CreateLogger(type);

        return Create(logger, name, activityKind);
    }

    /// <summary>
    /// Creates a new activity with the specified logger and identifier.
    /// </summary>
    /// <param name="logger">The logger instance to use for activity logging.</param>
    /// <param name="identify">A string identifier for the activity (e.g., operation name).</param>
    /// <param name="activityKind">The kind of activity to create (defaults to Internal).</param>
    /// <returns>An <see cref="IActivity"/> instance representing the created activity.</returns>
    private IActivity Create(ILogger logger, string identify, ActivityKind activityKind = ActivityKind.Internal)
    {
        Activity? activity;

        if (Activity.Current is null && _currentActivity is not null)
        {
            var activityContext = ActivityContext.Parse(_currentActivity.Id ?? "", _currentActivity.TraceStateString);
            activity = _mainSource.StartActivity(identify, activityKind, activityContext);
        }
        else
            activity = _mainSource.StartActivity(identify, activityKind);

        activity?.Start();

        ArgumentNullException.ThrowIfNull(activity);

        var verxActivity = new VerxActivity(logger, activity);
        verxActivity.SetTag("CorrelationId", correlation.Id);

        return verxActivity;
    }

    /// <summary>
    /// Gets the name of the type, including generic arguments if applicable.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> whose name should be retrieved.</param>
    /// <returns>
    /// A string representing the type's name. If the type is generic, the name will include its generic arguments.
    /// </returns>
    private static string GetName(Type type)
    {
        static string GetGenericTypeName(Type t)
        {
            var genericTypeName = t.GetGenericTypeDefinition().Name;

            var tickIndex = genericTypeName.IndexOf('`');
            return tickIndex > 0 ? genericTypeName[..tickIndex] : genericTypeName;
        }

        return type.IsGenericType
            ? $"{GetGenericTypeName(type)}<{string.Join(",", type.GetGenericArguments().Select(t => t.Name))}>"
            : type.Name;
    }
}
