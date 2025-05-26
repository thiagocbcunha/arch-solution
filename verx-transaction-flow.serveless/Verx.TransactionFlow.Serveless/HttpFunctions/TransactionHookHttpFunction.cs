using MediatR;
using System.Text.Json;
using Verx.Enterprise.Tracing;
using Verx.Enterprise.WebApplications;
using Google.Cloud.Functions.Framework;
using Verx.Authentication.Service.Application;

namespace Verx.TransactionFlow.Serveless.HttpFunctions;

public class TransactionHookHttpFunction(ILogger<TransactionHookHttpFunction> logger, IMediator mediator, ITracer tracer) : IHttpFunction
{
    public async Task HandleAsync(HttpContext context)
    {
        using var span = tracer.Span<TransactionHookHttpFunction>();
        try
        {
            span.NewMessage("Starting request.");
            logger.LogInformation("Starting request.");

            using var reader = new StreamReader(context.Request.Body);
            var createTransactionCommand = await JsonSerializer.DeserializeAsync(reader.BaseStream, ApplicationJsonSerialiazerContext.Default.CreateTransactionCommand);

            ArgumentNullException.ThrowIfNull(createTransactionCommand);

            var result = await mediator.Send(createTransactionCommand);

            span.Success();
            await context.Ok(result);
        }
        catch (Exception ex)
        {
            span.Failure(ex.Message);
            throw;
        }
    }
}