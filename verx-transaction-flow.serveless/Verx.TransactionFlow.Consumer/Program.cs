using Confluent.Kafka;
using RabbitMQ.Client;
using Verx.Enterprise.Tracing;
using Microsoft.Extensions.Hosting;
using Verx.Enterprise.MessageBroker;
using Verx.TransactionFlow.Application;
using Verx.TransactionFlow.Domain.Event;
using Microsoft.Extensions.Configuration;
using Verx.TransactionFlow.Domain.Options;
using Verx.TransactionFlow.Consumer.Consumers;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        config
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<TransactionFlowConsumerService>();

        services.AddHttpClient();
        services.AddApplication();

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

            if (!Uri.TryCreate(rabbitSettings.Host, UriKind.Absolute, out Uri uri))
                throw new ArgumentException("Invalid RabbitMQ host URI", nameof(rabbitSettings.Host));

            return new ConnectionFactory
            {
                Uri = uri,
                Port = rabbitSettings.Port,
                UserName = rabbitSettings.UserName,
                Password = rabbitSettings.Password,
                VirtualHost = rabbitSettings.VirtualHost
            };
        });

        services.AddKafkaConsumer(sp =>
        {
            var kafkaSettings = context.Configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>();
            ArgumentNullException.ThrowIfNull(kafkaSettings, "KafkaSettings cannot be null");
            return new ConsumerConfig
            {
                EnableAutoCommit = false,
                GroupId = kafkaSettings.ConsumerGroup,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                BootstrapServers = kafkaSettings.BootstrapServers,
            };
        });

        services.AddConsumerFor<TransationCreated>(BrokerType.Both);
    })
    .Build();

await host.RunAsync();