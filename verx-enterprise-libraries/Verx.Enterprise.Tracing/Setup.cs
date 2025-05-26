using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Verx.Enterprise.Tracing;

[ExcludeFromCodeCoverage]
public static class Setup
{
    public static TracingActionProviderBuilder TraceBuilder(this IServiceCollection service, Func<TracerExporterOptions> tracerExportOptionsFunc)
    {
        var tracerExOpt = tracerExportOptionsFunc();
        return new TracingActionProviderBuilder(service, tracerExOpt);
    }
}
