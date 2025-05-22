namespace Verx.Consolidated.Domain.Entities;

/// <summary>
/// Represents a financial transaction between two accounts.
/// </summary>
/// <remarks>
/// The <see cref="TransactionEntity"/> class encapsulates the details of a single financial transaction,
/// including the amount, currency, description, sender and receiver account identifiers, and the transaction date.
/// </remarks>
public class TransactionEntity : Entity<Guid>
{
    /// <summary>
    /// Gets the unique identifier for this transaction.
    /// </summary>
    public Guid TransactionId { get; set; }

    /// <summary>
    /// Gets the monetary amount involved in the transaction.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets the date and time when the transaction occurred.
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Gets the current event associated with this transaction.
    /// </summary>    
    public TransactionEventEntity CurrentEvent { get; private set; } = null!;

    /// <summary>
    /// Events associated with this transaction.
    /// </summary>
    public IEnumerable<TransactionEventEntity> TransactionEvents { get; init; } = [];

    /// <summary>
    /// Sets the current event for this transaction.
    /// </summary>
    /// <param name="transactionEvent"></param>
    public void SetCurrentEvent(TransactionEventEntity? transactionEvent)
    {
        if(transactionEvent is not null)
            CurrentEvent = transactionEvent;
    }
}
