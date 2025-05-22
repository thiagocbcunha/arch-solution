using MediatR;
using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Verx.TransactionFlow.Domain.Event;
using Verx.TransactionFlow.Common.Contracts;
using Verx.TransactionFlow.Application.ProcessEvent;
using Verx.TransactionFlow.Infrastructure.MessageBrokers;

namespace Verx.TransactionFlow.Consumer.Consumers;

/// <summary>
/// Represents a background service that consumes transaction flow events from a Kafka topic.
/// </summary>
/// <param name="logger"></param>
/// <param name="channel"></param>
/// <param name="activityFactory"></param>
public class TransactionFlowConsumerService
    (ILogger<TransactionFlowConsumerService> logger, Channel<EventMessage<TransationCreated>> channel, IActivityTracing activityFactory, IMediator mediator) : BackgroundService
{
    /// <summary>
    /// Executes the background service.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!await StartConsuming(stoppingToken))
        {
            logger.LogInformation("Kafka consumer service is restarting in 5 seconds.");
            await Task.Delay(5000, stoppingToken);
        }
    }

    /// <summary>
    /// Starts consuming messages from the Kafka topic.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    public async Task<bool> StartConsuming(CancellationToken cancellationToken)
    {
        logger.LogInformation($"Starting Kafka consumer background service.");

        try
        {
            await foreach (var evnet in channel.Reader.ReadAllAsync(cancellationToken))
            {
                using var envtActivity = activityFactory.Create<TransactionFlowConsumerService>(ActivityKind.Internal);
                try
                {
                    envtActivity.LogMessage($"Handler new message: {evnet.Message.TransactionId}");

                    var result = mediator.Send((TransactionCreatedEventCommand)evnet.Message, cancellationToken);

                    evnet.Ack();
                    envtActivity.Success();
                }
                catch (Exception ex)
                {
                    envtActivity.Failure(ex.Message);
                    throw;
                }
            }

            return true;
        }
        catch { }

        return false;
    }
}
