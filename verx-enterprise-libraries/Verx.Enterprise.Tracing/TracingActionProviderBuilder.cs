using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Verx.Enterprise.Tracing;


[ExcludeFromCodeCoverage]
public class TracingActionProviderBuilder
{
    private readonly IServiceCollection _service;
    private Action<TracerProviderBuilder> _providerBuilderAction = i => { };

    internal TracingActionProviderBuilder(IServiceCollection service, TracerExporterOptions tracerExporter)
    {
        _service = service;

        _service.AddScoped<ITracer>(sp => new Tracer(tracerExporter.ApplicationName, tracerExporter.Version));

        _providerBuilderAction += i => i
            .AddSource(tracerExporter.ApplicationName)
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: tracerExporter.ApplicationName, serviceVersion: tracerExporter.Version));

        _providerBuilderAction += i => i.AddAspNetCoreInstrumentation();
        _providerBuilderAction += i => i.AddOtlpExporter(options => options = tracerExporter);
        _providerBuilderAction += i => i.AddOtlpExporter(opts => opts.Endpoint = tracerExporter.Endpoint);
    }

    public TracingActionProviderBuilder AddSQLInstrumentation()
    {
        _providerBuilderAction += i => i.AddSqlClientInstrumentation(options => options.SetDbStatementForText = true);

        return this;
    }

    public TracingActionProviderBuilder AddEFInstrumentation()
    {
        _providerBuilderAction += i => i.AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true);

        return this;
    }

    public TracingActionProviderBuilder AddCustomInstrumentation(Action<TracerProviderBuilder> configureInstrumentation)
    {
        _providerBuilderAction += configureInstrumentation;

        return this;
    }

    public IServiceCollection Build()
    {
        _service.AddOpenTelemetry()
            .WithTracing(_providerBuilderAction);

        return _service;
    }
}
