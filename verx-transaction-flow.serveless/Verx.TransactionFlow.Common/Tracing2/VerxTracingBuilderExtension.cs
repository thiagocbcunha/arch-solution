using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Verx.TransactionFlow.Common.Tracing;

/// <summary>
/// This class provides extension methods for setting up OpenTelemetry tracing in an ASP.NET Core application.
/// </summary>
[ExcludeFromCodeCoverage]
public static class VerxTracingBuilderExtension
{
    /// <summary>
    /// Creates and returns a <see cref="VerxTracingBuilder"/> for configuring OpenTelemetry tracing.
    /// </summary>
    /// <param name="service">The service collection to register dependencies.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <returns>A <see cref="VerxTracingBuilder"/> instance for further tracing configuration.</returns>
    public static VerxTracingBuilder CreateEnterpriseTracingBuilder(this IServiceCollection service, IConfiguration configuration)
        => new(service, configuration);

    /// <summary>
    /// Adds basic OpenTelemetry tracing services to the application using the provided configuration.
    /// </summary>
    /// <param name="service">The service collection to register dependencies.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with tracing services registered.</returns>
    public static IServiceCollection AddBasicTrancing(this IServiceCollection service, IConfiguration configuration)
    {
        var tracingBuilder = service.CreateEnterpriseTracingBuilder(configuration);
        
        tracingBuilder.AddSQLInstrumentation();
        tracingBuilder.AddEFInstrumentation();

        tracingBuilder.BuildService();

        return service;
    }
}
