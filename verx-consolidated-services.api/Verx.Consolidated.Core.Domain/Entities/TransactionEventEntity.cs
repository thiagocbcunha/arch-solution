namespace Verx.Consolidated.Domain.Entities;

/// <summary>
/// Represents a financial transaction between two accounts.
/// </summary>
/// <remarks>
/// The <see cref="TransactionEventEntity"/> class encapsulates the details of a single financial transaction,
/// including the amount, currency, description, sender and receiver account identifiers, and the transaction date.
/// </remarks>
public class TransactionEventEntity(string? currency = null, string? description = null, string? senderAccountId = null, string? receiverAccountId = null) : Entity<Guid>
{
    private int _versionNum = 1;

    /// <summary>
    /// Gets the unique identifier for this transaction event.
    /// </summary>
    public int VersionNum { get; private set; } = 0;

    /// <summary>
    /// Gets the ISO currency code (e.g., "USD", "EUR") for the transaction amount.
    /// </summary>
    public string Currency { get; private set; } = currency!;

    /// <summary>
    /// Gets the description or purpose of the transaction.
    /// </summary>
    public string Description { get; private set; } = description!;

    /// <summary>
    /// Gets the unique identifier of the sender's account.
    /// </summary>
    public string SenderAccountId { get; private set; } = senderAccountId!;

    /// <summary>
    /// Gets the unique identifier of the receiver's account.
    /// </summary>
    public string ReceiverAccountId { get; private set; } = receiverAccountId!;

    /// <summary>
    /// Name of the system or user that created the transaction event.
    /// </summary>
    public string CreateBy { get; private set; } = null!;

    /// <summary>
    /// Gets the date and time when the transaction event was created.
    /// </summary>
    public DateTime CreateDate { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Checks if the transaction event has been modified since its creation.
    /// </summary>
    public bool IsChanged => VersionNum != _versionNum;

    /// <summary>
    /// Changes the version number of the transaction event.
    /// </summary>
    private void ChangeVersionNum()
    {
        VersionNum = _versionNum + 1;
        CreateDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Changes the currency of the transaction event.
    /// </summary>
    /// <param name="currency">The new ISO currency code.</param>
    public void ChangeCurrency(string currency)
    {
        Currency = currency;
        ChangeVersionNum();
    }

    /// <summary>
    /// Changes the description of the transaction event.
    /// </summary>
    /// <param name="description">The new description.</param>
    public void ChangeDescription(string description)
    {
        Description = description;
        ChangeVersionNum();
    }

    /// <summary>
    /// Changes the sender's account identifier for the transaction event.
    /// </summary>
    /// <param name="senderAccountId">The new sender account ID.</param>
    public void ChangeSenderAccountId(string senderAccountId)
    {
        SenderAccountId = senderAccountId;
        ChangeVersionNum();
    }

    /// <summary>
    /// Changes the receiver's account identifier for the transaction event.
    /// </summary>
    /// <param name="receiverAccountId">The new receiver account ID.</param>
    public void ChangeReceiverAccountId(string receiverAccountId)
    {
        ReceiverAccountId = receiverAccountId;
        ChangeVersionNum();
    }

    /// <summary>
    /// Changes the creator of the transaction event.
    /// </summary>
    /// <param name="createBy">The new creator name or system.</param>
    public void ChangeCreateBy(string createBy)
    {
        CreateBy = createBy;
        ChangeVersionNum();
    }
}
