using Serilog;
using Serilog.Filters;
using Serilog.Exceptions;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Verx.TransactionFlow.Common.Options;

namespace Verx.TransactionFlow.Common.Logging;

/// <summary>
/// VerxLoggerBuilder is a class that provides methods to configure Serilog logging.
/// </summary>
/// <param name="configuration"></param>
[ExcludeFromCodeCoverage]
internal class VerxLoggerBuilder(IConfiguration configuration)
{
    private readonly LoggerConfiguration _loggerconfiguration = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="VerxLoggerBuilder"/> class.
    /// </summary>
    /// <returns></returns>
    public VerxLoggerBuilder ApplyFilter()
    {
        _loggerconfiguration
            .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
            .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("Microsoft.AspNetCore")));

        return this;
    }

    /// <summary>
    /// Adds enrichment to the logger configuration.
    /// </summary>
    /// <returns></returns>
    public VerxLoggerBuilder AddEnrich()
    {
        _loggerconfiguration
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationId()
            .Enrich.WithExceptionDetails()
            .Enrich.WithCorrelationIdHeader();

        return this;
    }

    /// <summary>
    /// Adds a name to the logger configuration based on the provided configuration section.
    /// </summary>
    /// <param name="appNameConfigurationSection"></param>
    /// <returns></returns>
    public VerxLoggerBuilder AddName(string appNameConfigurationSection)
    {
        var applicationName = configuration.GetValue<string>(appNameConfigurationSection);

        _loggerconfiguration
            .Enrich.WithProperty("ApplicationName", applicationName);

        return this;
    }

    /// <summary>
    /// Writes logs to the console.
    /// </summary>
    /// <returns></returns>
    public VerxLoggerBuilder WriteToConsole()
    {
        _loggerconfiguration.WriteTo.Async(wt => wt.Console());
        return this;
    }

    /// <summary>
    /// Writes logs to Logstash using the specified endpoint from the configuration.
    /// </summary>
    /// <returns></returns>
    public VerxLoggerBuilder WriteToLogstash()
    {
        var logSettings = new ObservabilitySettings();
        configuration.GetRequiredSection(nameof(ObservabilitySettings)).Bind(logSettings);
        _loggerconfiguration.WriteTo.Http(logSettings.LogstashEndpoint, queueLimitBytes: null);
        return this;
    }

    /// <summary>
    /// Builds the logger configuration and creates a logger instance.
    /// </summary>
    /// <returns></returns>
    public ILogger Build()
    {
        return _loggerconfiguration.CreateLogger();
    }
}