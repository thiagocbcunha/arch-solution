using MediatR;
using Microsoft.AspNetCore.Mvc;
using Verx.Consolidated.Common.Contracts;
using Verx.Consolidated.Application.CreateTransation;

namespace Verx.Consolidated.Onboarding.Command.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ConsolidatedController(IMediator mediator, IActivityTracing activityFactory) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateTransactionCommand createTransaction)
    {
        using var activity = activityFactory.Create($"Create-Person");
        activity.SetTag("MathodName", "Post");
        activity.LogMessage($"Executing: Person method:Post");

        await mediator.Send(createTransaction);

        return CreatedAtAction(nameof(Post), new { id = createTransaction.TransactionId }, createTransaction);
    }
}