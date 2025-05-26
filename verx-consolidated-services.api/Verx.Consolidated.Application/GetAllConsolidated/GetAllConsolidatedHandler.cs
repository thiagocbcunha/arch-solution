using MediatR;
using Verx.Enterprise.Tracing;
using Microsoft.Extensions.Logging;
using Verx.Consolidated.Domain.Contracts;

namespace Verx.Consolidated.Application.GetAllConsolidated;

public class GetAllConsolidatedHandler(ILogger<GetAllConsolidatedHandler> logger,  ITracer tracer, IConsolidatedNSqlRepository consolidatedNSqlRepository) : IRequestHandler<GetAllConsolidatedCommand, GetAllConsolidatedResult>
{
    public Task<GetAllConsolidatedResult> Handle(GetAllConsolidatedCommand request, CancellationToken cancellationToken)
    {
        using var activity = tracer.Span<GetAllConsolidatedHandler>();

        activity.NewMessage($"Getting all consolidated transactions");
        logger.LogInformation("Getting all consolidated transactions: {request}", request);

        var consolidatedDtos = consolidatedNSqlRepository.GetMany(x => x.Date == request.Date);

        activity.Success();

        return Task.FromResult(new GetAllConsolidatedResult(consolidatedDtos));
    }
}