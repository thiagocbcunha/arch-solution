using System.Diagnostics;
using OpenTelemetry.Context.Propagation;

namespace Verx.Enterprise.Tracing;

public interface ITracer
{
    ISpan Span<TCaller>();

    ISpan RootSpan<TCaller>(ActivityKind activityKind, ActivityContext? activityContext = null);

    void AddParentContext(PropagationContext parentContext);
}
