using Microsoft.Extensions.Hosting;
using Verx.TransactionFlow.Application;
using Verx.TransactionFlow.Domain.Event;
using Microsoft.Extensions.Configuration;
using Verx.TransactionFlow.Infrastructure;
using Verx.TransactionFlow.Common.Tracing;
using Verx.TransactionFlow.Consumer.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Verx.TransactionFlow.Infrastructure.MessageBrokers.RabbitMQ;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());

        var environmentName = hostingContext.HostingEnvironment.EnvironmentName;

        config
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

        config.AddJsonFile("appsettings.json", optional: false);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<TransactionFlowConsumerService>(); 
        
        services.AddHttpClient();
        services.AddApplication();
        services.AddEventMessageCorrelation();
        services.AddKafkaProducerBy<TransationCreated>();
        services.AddKafkaConsumerBy<TransationCreated>();
        services.AddInfrastructure(context.Configuration);
        services.AddBasicTrancingHostedService<TransactionFlowConsumerService>(context.Configuration);

        services.RabbitBuilder(context.Configuration)
            .AddRabbitConsumer<TransationCreated>()
            .AddRabbitProducer()
            .Build();

    })
    .Build();

await host.RunAsync();