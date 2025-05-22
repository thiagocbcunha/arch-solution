using MediatR;
using Verx.Consolidated.Common.Contracts;
using Verx.Consolidated.Domain.Contracts;

namespace Verx.Consolidated.Application.GetAllConsolidated;

/// <summary>
/// Handler for the CreateUserCommand.
/// </summary>
/// <param name="userRegistrationService"></param>
public class GetAllConsolidatedHandler(IActivityTracing activityTracing, IConsolidatedNSqlRepository consolidatedNSqlRepository) : IRequestHandler<GetAllConsolidatedCommand, GetAllConsolidatedResult>
{
    /// <summary>
    /// Handles the CreateUserCommand.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<GetAllConsolidatedResult> Handle(GetAllConsolidatedCommand request, CancellationToken cancellationToken)
    {
        using var activity = activityTracing.Create<GetAllConsolidatedHandler>();

        activity.LogMessage($"Getting all consolidated transactions");

        var consolidatedDtos = consolidatedNSqlRepository.GetMany(x => x.Date == request.Date);

        activity.Success();

        return new GetAllConsolidatedResult(consolidatedDtos);
    }
}