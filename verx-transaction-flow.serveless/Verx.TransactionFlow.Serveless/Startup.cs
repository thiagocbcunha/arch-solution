using Verx.TransactionFlow.Serveless;
using Google.Cloud.Functions.Hosting;
using Verx.TransactionFlow.Application;
using Verx.TransactionFlow.Domain.Event;
using Verx.TransactionFlow.Common.Logging;
using Verx.TransactionFlow.Common.Tracing;
using Verx.TransactionFlow.Infrastructure;
using Verx.TransactionFlow.Serveless.Common;
using Verx.TransactionFlow.Infrastructure.MessageBrokers.RabbitMQ;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Verx.TransactionFlow.Serveless;

public class Startup : FunctionsStartup
{
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        services.AddKafkaProducerBy<TransationCreated>();

        services.AddHttpClient();
        services.AddApplication();
        services.AddHttpContextAccessor();
        services.AddHttpHeaderCorrelation();
        services.AddBasicTracing(context.Configuration);
        services.AddInfrastructure(context.Configuration);

        services
            .RabbitBuilder(context.Configuration)
            .AddRabbitProducer()
            .Build();
    }

    public override void ConfigureLogging(WebHostBuilderContext context, ILoggingBuilder logging)
    {
        logging.ConfigureEnterpriceLog(context.Configuration, "ApplicationName");
    }

    public override void Configure(WebHostBuilderContext context, IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}