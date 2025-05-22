using MediatR;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Verx.TransactionFlow.Common.Contracts;
using System.Net.Http.Json;
using MassTransit.Configuration;
using Microsoft.Extensions.Options;
using Verx.TransactionFlow.Domain.Options;
using Verx.TransactionFlow.Domain.Event;

namespace Verx.TransactionFlow.Application.ProcessEvent;

/// <summary>
/// Handler for the CreateUserCommand.
/// </summary>
/// <param name="userRegistrationService"></param>
public class TransactionCreatedEventHandler(ILogger<TransactionCreatedEventHandler> logger, IOptions<ConsolidatedSettings> options, IActivityTracing activityFactory, IHttpClientFactory httpClientFactory) : IRequestHandler<TransactionCreatedEventCommand, TransactionCreatedEventResult>
{
    public async Task<TransactionCreatedEventResult> Handle(TransactionCreatedEventCommand request, CancellationToken cancellationToken)
    {
        using var activity = activityFactory.Create<TransactionCreatedEventHandler>(ActivityKind.Internal);
        activity.LogMessage("Processing TransactionCreated Event");

        var httpClient = httpClientFactory.CreateClient();

        // Serialize the request object to JSON content
        var jsonContent = JsonContent.Create((TransationCreated)request);
        var response = await httpClient.PostAsync($"{options.Value.UrlBase}/Consolidated", jsonContent, cancellationToken);

        response.EnsureSuccessStatusCode();

        return new TransactionCreatedEventResult(true);
    }
}
