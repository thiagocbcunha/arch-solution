using MediatR;
using Verx.TransactionFlow.Domain.Event;
using Verx.TransactionFlow.Domain.Contracts;
using Verx.TransactionFlow.Common.Contracts;

namespace Verx.TransactionFlow.Application.CreateTransation;

/// <summary>
/// Handler for the CreateUserCommand.
/// </summary>
/// <param name="userRegistrationService"></param>
public class CreateTransactionHandler(IActivityTracing activityTracing, IKafkaProducer<TransationCreated> producer) : IRequestHandler<CreateTransactionCommand, CreateTransactionResult>
{
    /// <summary>
    /// Handles the CreateUserCommand.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CreateTransactionResult> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        using var activity = activityTracing.Create<CreateTransactionHandler>();

        activity.LogMessage($"Creating transaction with ID: {request.TransactionId}");

        request.TransactionId = Guid.NewGuid();
        request.TransactionDate = DateTime.UtcNow;

        await producer.SendMessageAsync(request, cancellationToken);

        activity.Success();

        return new CreateTransactionResult(true);
    }
}