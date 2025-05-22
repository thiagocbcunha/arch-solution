using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Verx.Consolidated.Common.Tracing;

/// <summary>
/// Provides extension methods for configuring and registering OpenTelemetry tracing services
/// in a .NET application using dependency injection.
/// </summary>
/// <remarks>
/// <para>
/// This static class offers convenient extension methods for <see cref="IServiceCollection"/> to
/// set up tracing infrastructure based on application configuration. It leverages the
/// <see cref="VerxActivityTracingBuilder"/> to encapsulate the setup of tracing sources,
/// resource information, and instrumentation for HTTP, ASP.NET Core, SQL, and Entity Framework.
/// </para>
/// <para>
/// Typical usage involves calling <see cref="AddBasicTracing"/> in your application's
/// service registration pipeline, passing in the application's configuration.
/// </para>
/// </remarks>
[ExcludeFromCodeCoverage]
public static class VerxActivityTracingBuilderExtension
{
    /// <summary>
    /// Creates a new <see cref="VerxActivityTracingBuilder"/> instance for configuring tracing services.
    /// </summary>
    /// <param name="service">The service collection for dependency injection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>
    /// A <see cref="VerxActivityTracingBuilder"/> that can be used to further configure tracing options.
    /// </returns>
    /// <example>
    /// <code>
    /// var builder = services.TracingBuilder(configuration);
    /// builder.AddSQLInstrumentation();
    /// builder.BuildService();
    /// </code>
    /// </example>
    public static VerxActivityTracingBuilder TracingBuilder(this IServiceCollection service, IConfiguration configuration)
        => new(service, configuration);

    /// <summary>
    /// Adds basic OpenTelemetry tracing services to the application's service collection.
    /// </summary>
    /// <param name="service">The service collection for dependency injection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling method chaining.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method initializes the tracing builder and registers the default tracing services,
    /// including HTTP, ASP.NET Core, and OTLP exporter instrumentations. For additional
    /// instrumentations (such as SQL or Entity Framework), use the <see cref="TracingBuilder"/>
    /// method and configure as needed before calling <c>BuildService()</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// services.AddBasicTracing(configuration);
    /// </code>
    /// </example>
    public static IServiceCollection AddBasicTracing(this IServiceCollection service, IConfiguration configuration)
    {
        var builder = service.TracingBuilder(configuration);
        builder.BuildService();

        return service;
    }

    /// <summary>
    /// Adds a hosted service that utilizes OpenTelemetry tracing to the application's service collection.
    /// </summary>
    /// <typeparam name="THostedService"></typeparam>
    /// <param name="service"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddBasicTrancingHostedService<THostedService>(this IServiceCollection service, IConfiguration configuration)
        where THostedService : class
    {
        var builder = service.TracingBuilder(configuration);
        builder.BuildService();
        return service;
    }
}
