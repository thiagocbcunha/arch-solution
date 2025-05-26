using MediatR;
using System.Net.Http.Json;
using Verx.Enterprise.Tracing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Verx.TransactionFlow.Domain.Event;
using Verx.TransactionFlow.Domain.Options;

namespace Verx.TransactionFlow.Application.ProcessEvent;


/// <summary>
/// Handles the processing of TransactionCreated events by sending the event data
/// to the configured consolidated service endpoint.
/// </summary>
/// <param name="logger">Logger instance for logging information and errors.</param>
/// <param name="options">Options containing consolidated service settings.</param>
/// <param name="tracer">Tracer for distributed tracing and diagnostics.</param>
/// <param name="httpClientFactory">Factory to create HttpClient instances.</param>
public class TransactionCreatedEventHandler(ILogger<TransactionCreatedEventHandler> logger, IOptions<ConsolidatedSettings> options, ITracer tracer, IHttpClientFactory httpClientFactory) : IRequestHandler<TransactionCreatedEventCommand, TransactionCreatedEventResult>
{
    /// <summary>
    /// Handles the TransactionCreatedEventCommand by posting the event data to the consolidated service.
    /// </summary>
    /// <param name="request">The transaction created event command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result indicating the success of the operation.</returns>
    public async Task<TransactionCreatedEventResult> Handle(TransactionCreatedEventCommand request, CancellationToken cancellationToken)
    {
        using var span = tracer.Span<TransactionCreatedEventHandler>();
        span.NewMessage("Processing TransactionCreated Event");

        var httpClient = httpClientFactory.CreateClient();

        var jsonContent = JsonContent.Create((TransationCreated)request);
        var response = await httpClient.PostAsync($"{options.Value.UrlBase}/Consolidated", jsonContent, cancellationToken);

        response.EnsureSuccessStatusCode();

        return new TransactionCreatedEventResult(true);
    }
}
