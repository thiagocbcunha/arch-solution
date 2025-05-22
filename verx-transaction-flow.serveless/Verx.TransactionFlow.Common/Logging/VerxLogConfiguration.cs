using Serilog;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace Verx.TransactionFlow.Common.Logging;

/// <summary>
/// Provides extension methods for configuring enterprise-level logging using Serilog within the application.
/// </summary>
[ExcludeFromCodeCoverage]
public static class VerxLogConfiguration
{
    /// <summary>
    /// Configures the logging pipeline for the application using Serilog and custom Verx logger settings.
    /// This method clears all existing logging providers, applies enrichment, filtering, and output sinks
    /// (such as console and Logstash), and sets the application name for log context.
    /// </summary>
    /// <param name="loggingBuilder">The <see cref="ILoggingBuilder"/> to configure.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance containing logging settings.</param>
    /// <param name="appNameSection">The configuration section or value representing the application name for logging context.</param>
    /// <remarks>
    /// This method is intended to be called during application startup, typically in the Program.cs or Startup.cs file.
    /// It leverages a custom <c>VerxLoggerBuilder</c> to set up Serilog with additional enrichers, filters, and sinks.
    /// </remarks>
    public static void ConfigureEnterpriceLog(this ILoggingBuilder loggingBuilder, IConfiguration configuration, string appNameSection)
    {
        var logBulder = new VerxLoggerBuilder(configuration);

        loggingBuilder.ClearProviders();

        logBulder
            .AddEnrich()         
            .ApplyFilter()       
            .WriteToConsole()    
            .WriteToLogstash()   
            .AddName(appNameSection);

        loggingBuilder.AddSerilog(logBulder.Build());
    }
}
