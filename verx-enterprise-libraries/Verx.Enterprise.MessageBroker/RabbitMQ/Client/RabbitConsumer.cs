using Serilog.Context;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Verx.Enterprise.MessageBroker.RabbitMQ.Extensions;

namespace Verx.Enterprise.MessageBroker.RabbitMQ.Client;

[ExcludeFromCodeCoverage]
public class RabbitConsumer<TMessage>(ILogger<RabbitConsumer<TMessage>> _logger, IConnection _connection, IServiceProvider _provider, Channel<EventPackage<TMessage>> _channel) : BackgroundService
    where TMessage : class, new()
{
    private readonly IModel _model = _connection.CreateModel();
    private readonly Topology _topology = new(typeof(TMessage));

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting RabbitMQ consumer for {Name}.", typeof(TMessage).Name);
        _model.ConfigureTopoligy(_topology);

        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_model);
        consumer.Received += async (_, eventMessage) =>
        {
            var correlation = eventMessage.BasicProperties.CorrelationId();
            var parentContext = eventMessage.BasicProperties.ExtractContext();

            using var scope = _provider.CreateScope();
            var tracer = scope.ServiceProvider.GetRequiredService<InternalTraceFacade>();

            using (LogContext.PushProperty("CorrelationId", correlation))
            {
                _logger.LogInformation($"Received message: {eventMessage.DeliveryTag} - {eventMessage.RoutingKey}");

                using var span = tracer.CreateConsumerSpan<RabbitConsumer<TMessage>, TMessage>(parentContext, correlation, "rabbitmq");
                await _channel.Writer.WriteAsync(new EventPackage<TMessage> { RabbitMessage = eventMessage, ParentContext = parentContext }, stoppingToken);

                span.Success();
                _model.BasicAck(eventMessage.DeliveryTag, false);
            }

            await Task.CompletedTask;
        };

        _model.BasicConsume(_topology.Queue, false, consumer);

        return Task.CompletedTask;
    }
}