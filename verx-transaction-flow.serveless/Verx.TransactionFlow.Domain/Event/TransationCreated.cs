namespace Verx.TransactionFlow.Domain.Event;

public class TransationCreated
{
    public Guid TransactionId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string SenderAccountId { get; init; } = string.Empty;
    public string ReceiverAccountId { get; init; } = string.Empty;
    public DateTime TransactionDate { get; init; } = DateTime.UtcNow;
}