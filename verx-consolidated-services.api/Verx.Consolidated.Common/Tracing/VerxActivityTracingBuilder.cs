using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Verx.Consolidated.Common.Options;
using Verx.Consolidated.Common.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Verx.Consolidated.Common.Tracing;

/// <summary>
/// Provides a builder for configuring and registering OpenTelemetry tracing services
/// within a .NET application. This class encapsulates the setup of tracing sources,
/// resource information, and instrumentation for HTTP, ASP.NET Core, SQL, and Entity Framework.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="VerxActivityTracingBuilder"/> is intended to be used internally to
/// configure tracing based on application configuration and dependency injection.
/// </para>
/// <para>
/// It binds <see cref="ObservabilitySettings"/> from configuration, registers tracing
/// services, and allows optional instrumentation for SQL and Entity Framework.
/// </para>
/// </remarks>
[ExcludeFromCodeCoverage]
public class VerxActivityTracingBuilder
{
    private readonly IServiceCollection _service;
    private readonly IConfiguration _configuration;
    private Action<TracerProviderBuilder> _providerBuilderAction = i => { };

    /// <summary>
    /// Initializes a new instance of the <see cref="VerxActivityTracingBuilder"/> class.
    /// Binds observability settings from configuration, registers tracing services,
    /// and configures default instrumentations (HTTP, ASP.NET Core, OTLP exporter).
    /// </summary>
    /// <param name="service">The service collection for dependency injection.</param>
    /// <param name="configuration">The application configuration.</param>
    internal VerxActivityTracingBuilder(IServiceCollection service, IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;

        var enterpriseConfiguration = new ObservabilitySettings();
        _configuration.GetRequiredSection(nameof(ObservabilitySettings)).Bind(enterpriseConfiguration);

        Console.WriteLine(JsonSerializer.Serialize(enterpriseConfiguration));

        _service.AddScoped(i => enterpriseConfiguration);
        _service.AddScoped<IActivityTracing, VerxActivityTracing>();

        _providerBuilderAction += i => i
            .AddSource(enterpriseConfiguration.Name)
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: enterpriseConfiguration.Name, serviceVersion: enterpriseConfiguration.Version));

        //_providerBuilderAction += i => i.AddHttpClientInstrumentation();
        _providerBuilderAction += i => i.AddAspNetCoreInstrumentation();
        _providerBuilderAction += i => i.AddOtlpExporter(opts => opts.Endpoint = new Uri(enterpriseConfiguration.OTELEndpoint));
    }

    /// <summary>
    /// Adds SQL client instrumentation to the tracing pipeline.
    /// This enables capturing SQL command text in traces.
    /// </summary>
    public void AddSQLInstrumentation()
    {
        _providerBuilderAction += i => i.AddSqlClientInstrumentation(options => options.SetDbStatementForText = true);
    }

    /// <summary>
    /// Adds Entity Framework Core instrumentation to the tracing pipeline.
    /// This enables capturing EF Core command text in traces.
    /// </summary>
    public void AddEFInstrumentation()
    {
        _providerBuilderAction += i => i.AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true);
    }

    /// <summary>
    /// Finalizes the tracing configuration and registers OpenTelemetry tracing
    /// with all specified instrumentations and exporters.
    /// </summary>
    public void BuildService()
    {
        _service.AddOpenTelemetry().WithTracing(_providerBuilderAction);
    }
}
