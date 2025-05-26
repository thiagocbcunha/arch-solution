using Verx.Enterprise.Tracing;
using Verx.Enterprise.MessageBroker;
using Verx.Consolidated.Domain.Dtos;
using Verx.Consolidated.Domain.Contracts;

namespace Verx.Consolidated.Worker.Processors;

public class ConsolidatedEventProcessor(ILogger<ConsolidatedEventProcessor> logger, ITracer tracer, IChannel<ConsolidatedDto> channel, IConsolidatedNSqlRepository consolidatedNSqlRepositor)
    : IProcessors<ConsolidatedDto>
{
    public async Task<bool> ProcessingEvents(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting to consume messages");

        try
        {
            await foreach (var consolidatedEvent in channel.ConsumeAsync(cancellationToken))
            {
                using var span = tracer.Span<ConsolidatedEventProcessor>();
                try
                {
                    if (consolidatedEvent is not null)
                    {
                        span.NewMessage($"Consuming consolidated transaction with ID: {consolidatedEvent.Id}");
                        consolidatedNSqlRepositor.Update(consolidatedEvent);
                    }

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
