using MediatR;
using Verx.Enterprise.Tracing;
using Microsoft.Extensions.Logging;
using Verx.TransactionFlow.Domain.Event;
using Verx.Enterprise.MessageBroker.Kafka;

namespace Verx.TransactionFlow.Application.CreateTransation;

/// <summary>
/// Handles the creation of a new transaction by generating a new transaction ID and date,
/// publishing a <see cref="TransationCreated"/> event to Kafka, and returning the result.
/// </summary>
/// <remarks>
/// This handler uses distributed tracing and logging for observability, and ensures that
/// each transaction is uniquely identified and timestamped before being published.
/// </remarks>
public class CreateTransactionHandler(ILogger<CreateTransactionHandler> logger, ITracer activityTracing, IKafkaProducer<TransationCreated> producer) : IRequestHandler<CreateTransactionCommand, CreateTransactionResult>
{
    /// <summary>
    /// Handles the CreateUserCommand.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CreateTransactionResult> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        using var span = activityTracing.Span<CreateTransactionHandler>();

        span.NewMessage($"Creating transaction with ID: {request.TransactionId}");
        logger.LogInformation("Creating transaction with ID: {TransactionId}", request.TransactionId);

        request.TransactionId = Guid.NewGuid();
        request.TransactionDate = DateTime.UtcNow;

        await producer.PublishAsync(request, cancellationToken);

        span.Success();

        return new CreateTransactionResult(true);
    }
}