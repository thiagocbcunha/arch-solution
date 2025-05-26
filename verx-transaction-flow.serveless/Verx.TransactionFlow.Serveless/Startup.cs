using RabbitMQ.Client;
using Confluent.Kafka;
using Verx.Enterprise.Tracing;
using Verx.Enterprise.Logging;
using Verx.Enterprise.MessageBroker;
using Verx.TransactionFlow.Serveless;
using Google.Cloud.Functions.Hosting;
using Verx.TransactionFlow.Application;
using Verx.Enterprise.WebApplications;
using Verx.TransactionFlow.Domain.Options;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Verx.TransactionFlow.Serveless;

public class Startup : FunctionsStartup
{
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        var tracerBuilder = services.TraceBuilder(() =>
        {
            var applicationName = context.Configuration.GetValue<string>("ApplicationName") ?? "Verx.TransactionFlow.Serveless";
            var obsevabilitySettings = context.Configuration.GetSection(nameof(ObservabilitySettings)).Get<ObservabilitySettings>();

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

        services.AddRabbit(sp =>
        {
            var rabbitSettings = context.Configuration.GetSection(nameof(RabbitSettings)).Get<RabbitSettings>();

            ArgumentNullException.ThrowIfNull(rabbitSettings, "RabbitSettings cannot be null");

            return new ConnectionFactory
            {
                Port = rabbitSettings.Port,
                Uri = new Uri(rabbitSettings.Host),
                UserName = rabbitSettings.UserName,
                Password = rabbitSettings.Password,
                VirtualHost = rabbitSettings.VirtualHost
            };
        });

        services.AddKafkaProducer(sp =>
        {
            var kafkaSettings = context.Configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>();
            ArgumentNullException.ThrowIfNull(kafkaSettings, "KafkaSettings cannot be null");
            return new ProducerConfig
            {
                LingerMs = 5,
                Acks = Acks.All,
                BatchSize = 32 * 1024,
                EnableIdempotence = true,
                CompressionType = CompressionType.Gzip,
                BootstrapServers = kafkaSettings.BootstrapServers,
            };
        });

        services.AddHttpClient();
        services.AddApplication();
        services.AddHttpContextAccessor();
    }

    public override void ConfigureLogging(WebHostBuilderContext context, ILoggingBuilder logging)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        context.Configuration = configurationBuilder.Build();

        logging.ConfigureLogging(() =>
        {
            var applicationName = context.Configuration.GetValue<string>("ApplicationName") ?? "Verx.TransactionFlow.Serveless";
            var logstashEndpoint = context.Configuration.GetValue<string>("LogstashEndpoint") ?? "";

            return new Enterprise.Logging.LoggerFactory
            {
                ApplicationName = applicationName,
                LogstashEndpoint = logstashEndpoint
            };
        });
    }

    public override void Configure(WebHostBuilderContext context, IApplicationBuilder app)
    {
        app.ConfigureWebApplication();
    }
}