using Serilog;
using Verx.Enterprise.Correlation;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Verx.Enterprise.Logging;

[ExcludeFromCodeCoverage]
public static class Setup
{
    public static void ConfigureLogging(this ILoggingBuilder loggingBuilder, Func<LoggerFactory> loggerFactory)
    {
        var factory = loggerFactory();
        loggingBuilder.ClearProviders();
        loggingBuilder.AddSerilog(factory.Create());
        loggingBuilder.Services.TryAddCorrelation();
    }
}
