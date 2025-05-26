using Verx.Consolidated.Domain.Dtos;
using Verx.Consolidated.Domain.Contracts;

namespace Verx.Consolidated.Worker;

public class Worker(ILogger<Worker> logger, IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!await StartConsuming(stoppingToken))
        {
            logger.LogDebug("Retrying to consume messages from Kafka topic in 5 second...");
            await Task.Delay(5000, stoppingToken);
        }
    }

    public Task<bool> StartConsuming(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting to consume messages");

        using var scope = serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IProcessors<ConsolidatedDto>>().ProcessingEvents(cancellationToken);
    }
}
