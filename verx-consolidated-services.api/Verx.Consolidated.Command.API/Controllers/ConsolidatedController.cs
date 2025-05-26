using MediatR;
using Verx.Enterprise.Tracing;
using Microsoft.AspNetCore.Mvc;
using Verx.Consolidated.Application.CreateTransation;

namespace Verx.Consolidated.Onboarding.Command.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ConsolidatedController(IMediator mediator, ITracer tracer) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateTransactionCommand createTransaction)
    {
        using var activity = tracer.Span<ConsolidatedController>();
        activity.SetKey("MathodName", "Post");

        await mediator.Send(createTransaction);

        return CreatedAtAction(nameof(Post), new { id = createTransaction.TransactionId }, createTransaction);
    }
}