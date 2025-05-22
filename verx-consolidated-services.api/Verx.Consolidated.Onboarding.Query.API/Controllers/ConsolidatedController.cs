using MediatR;
using Microsoft.AspNetCore.Mvc;
using Verx.Consolidated.Domain.Dtos;
using Verx.Consolidated.Common.Contracts;
using Verx.Consolidated.Application.GetAllConsolidated;

namespace Verx.Consolidated.Onboarding.Query.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ConsolidatedController(IActivityTracing activityFactory, IMediator mediator) : ControllerBase
{
    [HttpGet("list")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<ConsolidatedDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        using var activity = activityFactory.Create($"GetAll");
        activity.SetTag("MathodName", "GetAll");
        activity.LogMessage($"Executing: Consolidated method:GetAll");

        var result = await mediator.Send(new GetAllConsolidatedCommand());

        return CreatedAtAction(nameof(GetAll), result.Consolidateds);
    }
}