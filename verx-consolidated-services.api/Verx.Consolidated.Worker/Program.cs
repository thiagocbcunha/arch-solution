using Verx.Enterprise.Tracing;
using Verx.Consolidated.Worker;
using Verx.Consolidated.Infra.IoC;
using Verx.Consolidated.Domain.Dtos;
using Verx.Enterprise.MessageBroker;
using Verx.Consolidated.Domain.Options;
using Verx.Consolidated.Domain.Contracts;
using Verx.Consolidated.Worker.Processors;

var builder = Host.CreateApplicationBuilder(args);

var environmentName = builder.Environment.EnvironmentName;

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.SetupApplication(builder.Configuration);

var tracerBuilder = builder.Services.TraceBuilder(() =>
{
    var applicationName = builder.Configuration.GetValue<string>("ApplicationName") ?? "Verx.Consolidated.Worker";
    var obsevabilitySettings = builder.Configuration.GetSection(nameof(ObservabilitySettings)).Get<ObservabilitySettings>();

    ArgumentNullException.ThrowIfNull(obsevabilitySettings, "ObservabilitySettings cannot be null");

    return new TracerExporterOptions
    {
        ApplicationName = applicationName,
        Version = obsevabilitySettings.Version,
        Endpoint = new Uri(obsevabilitySettings.OTELEndpoint),
    };
});

builder.Services.AddConsumerFor<ConsolidatedDto>(BrokerType.Rabbit);
builder.Services.AddScoped<IProcessors<ConsolidatedDto>, ConsolidatedEventProcessor>();

tracerBuilder
    .AddEFInstrumentation()
    .AddSQLInstrumentation()
    .Build();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
