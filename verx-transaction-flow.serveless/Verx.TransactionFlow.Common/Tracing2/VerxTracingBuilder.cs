using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Verx.TransactionFlow.Common.Options;
using Verx.TransactionFlow.Common.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Verx.TransactionFlow.Common.Tracing;

/// <summary>
/// This class is used to configure OpenTelemetry tracing for the application.
/// </summary>
[ExcludeFromCodeCoverage]
public class VerxTracingBuilder
{
    private readonly IServiceCollection _service;
    private readonly IConfiguration _configuration;
    Action<TracerProviderBuilder> _providerBuilderAction = i => { };

    /// <summary>
    /// Initializes a new instance of the <see cref="VerxTracingBuilder"/> class.
    /// Configures OpenTelemetry tracing and registers required services and settings.
    /// </summary>
    /// <param name="service">The service collection to register dependencies.</param>
    /// <param name="configuration">The application configuration instance.</param>
    internal VerxTracingBuilder(IServiceCollection service, IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;

        var enterpriseConfiguration = new ObservabilitySettings();
        _configuration.GetRequiredSection(nameof(ObservabilitySettings)).Bind(enterpriseConfiguration);

        _service.Configure<ObservabilitySettings>(_configuration.GetSection(nameof(ObservabilitySettings)));

        _service.AddScoped(i => enterpriseConfiguration);
        _service.AddScoped<ITelemetryActivity, VerxTelemetryActivity>();

        _providerBuilderAction += i => i
            .AddSource(enterpriseConfiguration.Name)
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: enterpriseConfiguration.Name, serviceVersion: enterpriseConfiguration.Version));

        _providerBuilderAction += i => i.AddHttpClientInstrumentation();
        _providerBuilderAction += i => i.AddAspNetCoreInstrumentation();
        _providerBuilderAction += i => i.AddOtlpExporter(opts => opts.Endpoint = new Uri(enterpriseConfiguration.OTELEndpoint));
    }

    /// <summary>
    /// Adds SQL client instrumentation to the OpenTelemetry tracing configuration.
    /// Enables capturing SQL statements for text commands.
    /// </summary>
    public void AddSQLInstrumentation()
    {
        _providerBuilderAction += i => i.AddSqlClientInstrumentation(options => options.SetDbStatementForText = true);
    }

    /// <summary>
    /// Adds Entity Framework Core instrumentation to the OpenTelemetry tracing configuration.
    /// Enables capturing SQL statements for text commands.
    /// </summary>
    public void AddEFInstrumentation()
    {
        _providerBuilderAction += i => i.AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true);
    }

    /// <summary>
    /// Builds and registers the OpenTelemetry tracing services using the configured instrumentation and exporters.
    /// </summary>
    public void BuildService()
    {
        _service
            .AddOpenTelemetry()
            .WithTracing(_providerBuilderAction);
    }
}