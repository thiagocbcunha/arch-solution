using Serilog;
using Serilog.Filters;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using System.Diagnostics.CodeAnalysis;

namespace Verx.Enterprise.Logging;

[ExcludeFromCodeCoverage]
internal class LoggerBuilder
{
    private readonly LoggerConfiguration _loggerconfiguration = new();

    public LoggerBuilder ApplyFilter()
    {
        _loggerconfiguration
            .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
            .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("Microsoft.AspNetCore")));

        return this;
    }

    public LoggerBuilder AddEnrich()
    {
        _loggerconfiguration
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationId()
            .Enrich.WithExceptionDetails()
            .Enrich.WithCorrelationIdHeader(headerKey: Correlation.Constants.CorrelationIdHeader);

        return this;
    }

    public LoggerBuilder AddName(string applicationName)
    {
        _loggerconfiguration
            .Enrich.WithProperty("ApplicationName", applicationName);

        return this;
    }

    public LoggerBuilder WriteToConsole()
    {
        _loggerconfiguration
            .WriteTo.Async(wt => wt.Console(new JsonFormatter()));

        return this;
    }
    public LoggerBuilder WriteToLogstash(string logstashEndpoint)
    {
        _loggerconfiguration.WriteTo.Http(logstashEndpoint, queueLimitBytes: null);
        return this;
    }

    public ILogger Build()
    {
        return _loggerconfiguration
            .CreateLogger();
    }
}