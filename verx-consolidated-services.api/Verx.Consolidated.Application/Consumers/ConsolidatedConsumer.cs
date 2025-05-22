using MassTransit;
using Verx.Consolidated.Domain.Dtos;
using Verx.Consolidated.Domain.Contracts;
using Verx.Consolidated.Common.Contracts;

namespace Verx.Consolidated.Application.Consumers;

public class ConsolidatedConsumer(IActivityTracing activityTracing, IConsolidatedNSqlRepository consolidatedNSqlRepository) : IConsumer<ConsolidatedDto>
{
    public async Task Consume(ConsumeContext<ConsolidatedDto> context)
    {
        using var activity = activityTracing.Create<ConsolidatedConsumer>();
        activity.LogMessage($"Consuming consolidated transaction with ID: {context.Message.Id}");
        consolidatedNSqlRepository.Update(context.Message);
        await Task.CompletedTask;
    }
}