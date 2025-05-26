using MediatR;
using Verx.Enterprise.Tracing;
using Microsoft.AspNetCore.Mvc;
using Verx.Consolidated.Domain.Dtos;
using Verx.Consolidated.Application.GetAllConsolidated;

namespace Verx.Consolidated.Onboarding.Query.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ConsolidatedController(ITracer tracer, IMediator mediator) : ControllerBase
{
    [HttpGet("list")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<IEnumerable<ConsolidatedDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        using var span = tracer.Span<ConsolidatedController>();

        span.SetKey("MathodName", "GetAll");

        var result = await mediator.Send(new GetAllConsolidatedCommand());

        span.Success();

        return CreatedAtAction(nameof(GetAll), result.Consolidateds);
    }
}