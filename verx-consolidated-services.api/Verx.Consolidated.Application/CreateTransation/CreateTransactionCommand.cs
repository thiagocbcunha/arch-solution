using MediatR;
using Verx.Consolidated.Domain.Entities;

namespace Verx.Consolidated.Application.CreateTransation;

/// <summary>
/// Represents a command to create a new transaction in the system.
/// This command encapsulates all necessary information required to create a transaction,
/// including transaction identifiers, monetary details, involved accounts, and metadata.
/// </summary>
/// <remarks>
/// This command is handled by a corresponding handler that processes the creation logic
/// and returns a <see cref="CreateTransactionResult"/> indicating the outcome.
/// </remarks>
public record CreateTransactionCommand : IRequest<CreateTransactionResult>
{
    /// <summary>
    /// Gets the unique identifier for the transaction.
    /// Defaults to a new GUID if not specified.
    /// </summary>
    public Guid TransactionId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets the amount of money to be transferred in the transaction.
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Gets the currency code (e.g., "USD", "EUR") for the transaction amount.
    /// </summary>
    public string Currency { get; init; } = string.Empty;

    /// <summary>
    /// Gets the description or memo for the transaction.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the unique identifier of the sender's account.
    /// </summary>
    public string SenderAccountId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the unique identifier of the receiver's account.
    /// </summary>
    public string ReceiverAccountId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the date and time when the transaction occurred.
    /// Defaults to the current UTC time if not specified.
    /// </summary>
    public DateTime TransactionDate { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Implicitly converts a <see cref="CreateTransactionCommand"/> to a <see cref="TransactionEntity"/> entity.
    /// </summary>
    /// <param name="command"></param>
    public static implicit operator TransactionEntity(CreateTransactionCommand command)
    {
        var transactionEntity = new TransactionEntity()
        {
            Amount = command.Amount,
            TransactionId = command.TransactionId,
            TransactionDate = command.TransactionDate,
        };

        transactionEntity.SetCurrentEvent(new TransactionEventEntity(command.Currency, command.Description, command.SenderAccountId, command.ReceiverAccountId));

        return transactionEntity;
    }
}
