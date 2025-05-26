using MediatR;
using Verx.Enterprise.Tracing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Verx.Enterprise.MessageBroker;
using Verx.TransactionFlow.Domain.Event;
using Verx.TransactionFlow.Application.ProcessEvent;

namespace Verx.TransactionFlow.Consumer.Consumers;

public class TransactionFlowConsumerService
    (ILogger<TransactionFlowConsumerService> logger, IChannel<TransationCreated> channel, ITracer tracer, IMediator mediator) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!await StartConsuming(stoppingToken))
        {
            logger.LogInformation("TransactionFlowConsumerService consumer service is restarting in 5 seconds.");
            await Task.Delay(5000, stoppingToken);
        }
    }

    public async Task<bool> StartConsuming(CancellationToken cancellationToken)
    {
        logger.LogInformation($"Starting Kafka consumer background service.");

        try
        {
            await foreach (var evnet in channel.ConsumeAsync(cancellationToken))
            {
                using var span = tracer.Span<TransactionFlowConsumerService>();
                try
                {
                    span.NewMessage($"Handler new message: {evnet.TransactionId}");
                    var result = await mediator.Send((TransactionCreatedEventCommand)evnet, cancellationToken);
                    span.Success();
                }
                catch (Exception ex)
                {
                    span.Failure(ex.Message);
                    await channel.RaiserError(span);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error consuming messages from Kafka topic.");
        }

        return false;
    }
}
