using MediatR;
using System.Text.Json;
using System.Diagnostics;
using Google.Cloud.Functions.Framework;
using Verx.TransactionFlow.Common.Contracts;
using Verx.TransactionFlow.Serveless.Common;
using Verx.Authentication.Service.Application;

namespace Verx.TransactionFlow.Serveless.HttpFunctions;

public class TransactionHookHttpFunction(IMediator mediator, IActivityTracing activityFactory) : IHttpFunction
{
    public async Task HandleAsync(HttpContext context)
    {
        using var activity = activityFactory.Start<TransactionHookHttpFunction>(ActivityKind.Server);
        try
        {
            activity.LogMessage("Starting request.");

            using var reader = new StreamReader(context.Request.Body);
            var createTransactionCommand = await JsonSerializer.DeserializeAsync(reader.BaseStream, ApplicationJsonSerialiazerContext.Default.CreateTransactionCommand);

            ArgumentNullException.ThrowIfNull(createTransactionCommand);

            var result = await mediator.Send(createTransactionCommand);

            await context.Ok(result);
            activity.Success();
        }
        catch (Exception ex)
        {
            activity.Failure(ex.Message);
            throw;
        }
    }
}