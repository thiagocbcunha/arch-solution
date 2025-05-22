using MassTransit;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Verx.TransactionFlow.Common.Logging;
using Verx.TransactionFlow.Domain.Contracts;

namespace Verx.TransactionFlow.Infrastructure.MessageBrokers.RabbitMQ;

/// <summary>
/// Provides an implementation of a message producer for RabbitMQ using MassTransit.
/// Responsible for sending messages to RabbitMQ, leveraging telemetry and logging for observability.
/// </summary>
/// <remarks>
/// This class uses dependency injection to receive required services such as logging, correlation, telemetry activity, and the MassTransit publish endpoint.
/// </remarks>
/// <param name="logger">The logger instance for logging message send operations.</param>
/// <param name="correlation">The correlation context for distributed tracing.</param>
/// <param name="activityFactory">The telemetry activity factory for tracing and diagnostics.</param>
/// <param name="ISendEndpointProvider">The MassTransit publish endpoint for sending messages to RabbitMQ.</param>
public class RabbitProducer(ILogger<RabbitProducer> logger, ICorrelation correlation, Common.Contracts.IActivityTracing activityFactory, ISendEndpointProvider sendEndpointProvider)
    : BaseProducer(correlation), IRabbitProducer
{
    // <inheritdoc />
    public async Task SendMessageAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
        where TMessage : class
    {
        using var activity = activityFactory.Create<RabbitProducer>(ActivityKind.Producer);
        activity.LogMessage("Sending Kafka Message");

        var eventMessge = CreateEventMessage(message);

        var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:{typeof(TMessage).Name.ToLower()}-exchange"));
        await endpoint.Send(eventMessge, cancellationToken);

        activity.Success();
    }
}
