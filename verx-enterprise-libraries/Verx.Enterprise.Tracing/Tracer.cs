using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using OpenTelemetry.Context.Propagation;

namespace Verx.Enterprise.Tracing;

[ExcludeFromCodeCoverage]
public class Tracer(string name, string? version = null) : ITracer
{
    private ActivityContext? _parentContext;
    public readonly ActivitySource _mainSource = new(name, version);

    /// <inheritdoc/>
    public ISpan Span<TCaller>()
    {
        Activity? activity;
        var name = GetName(typeof(TCaller));

        if (Activity.Current is null && _parentContext is not null)
            activity = _mainSource.StartActivity(name, ActivityKind.Internal, (ActivityContext)_parentContext);
        else
            activity = _mainSource.StartActivity(name);

        ArgumentNullException.ThrowIfNull(activity);

        return new Span(activity);
    }

    /// <inheritdoc/>
    public ISpan RootSpan<TCaller>(ActivityKind activityKind, ActivityContext? activityContext = null)
    {
        var name = GetName(typeof(TCaller));

        _parentContext = activityContext ?? _parentContext ?? default;

        var activity = _mainSource.StartActivity(name, activityKind, parentContext: (ActivityContext)_parentContext) ?? Activity.Current;

        ArgumentNullException.ThrowIfNull(activity);

        return new Span(activity);
    }

    public void AddParentContext(PropagationContext parentContext)
    {
        _parentContext = parentContext.ActivityContext;
    }

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
