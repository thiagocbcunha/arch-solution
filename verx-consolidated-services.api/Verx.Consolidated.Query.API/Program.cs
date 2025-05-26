using Verx.Enterprise.Tracing;
using Verx.Enterprise.Logging;
using Verx.Consolidated.Infra.IoC;
using Verx.Consolidated.Domain.Options;
using LoggerFactory = Verx.Enterprise.Logging.LoggerFactory;

var builder = WebApplication.CreateBuilder(args);

var environmentName = builder.Environment.EnvironmentName;

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.SetupApplication(builder.Configuration);

var tracerBuilder = builder.Services.TraceBuilder(() =>
{
    var applicationName = builder.Configuration.GetValue<string>("ApplicationName") ?? "Verx.Consolidated.Query.Api";
    var obsevabilitySettings = builder.Configuration.GetSection(nameof(ObservabilitySettings)).Get<ObservabilitySettings>();

    ArgumentNullException.ThrowIfNull(obsevabilitySettings, "ObservabilitySettings cannot be null");

    return new TracerExporterOptions
    {
        ApplicationName = applicationName,
        Version = obsevabilitySettings.Version,
        Endpoint = new Uri(obsevabilitySettings.OTELEndpoint),
    };
});

tracerBuilder
    .AddEFInstrumentation()
    .AddSQLInstrumentation()
    .Build();

builder.Logging.ConfigureLogging(() =>
{
    var logstashEndpoint = builder.Configuration.GetValue<string>("LogstashEndpoint") ?? "";
    var applicationName = builder.Configuration.GetValue<string>("ApplicationName") ?? "Verx.Consolidated.Query.Api";

    return new LoggerFactory
    {
        ApplicationName = applicationName,
        LogstashEndpoint = logstashEndpoint
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
