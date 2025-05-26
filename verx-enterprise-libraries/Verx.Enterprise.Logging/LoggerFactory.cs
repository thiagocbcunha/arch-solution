using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Verx.Enterprise.Logging;

[ExcludeFromCodeCoverage]
public class LoggerFactory
{
    public required string ApplicationName { get; set; }
    public required string LogstashEndpoint { get; set; }

    public ILogger Create() 
    {
        var loggerBuider = new LoggerBuilder();

        loggerBuider
            .AddEnrich()
            .ApplyFilter()
            .WriteToConsole()
            .WriteToLogstash(LogstashEndpoint)
            .AddName(ApplicationName);

        return loggerBuider.Build();
    }
}
