using MediatR;
using Verx.TransactionFlow.Domain.Event;

namespace Verx.TransactionFlow.Application.CreateTransation;

public record CreateTransactionCommand : IRequest<CreateTransactionResult>
{
    public Guid TransactionId { get; set; } = Guid.NewGuid();
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SenderAccountId { get; set; } = string.Empty;
    public string ReceiverAccountId { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    public static implicit operator TransationCreated(CreateTransactionCommand command)
       => new()
       {
           Amount = command.Amount,
           Currency = command.Currency,
           Description = command.Description,
           TransactionId = command.TransactionId,
           TransactionDate = command.TransactionDate,
           SenderAccountId = command.SenderAccountId,
           ReceiverAccountId = command.ReceiverAccountId
       };
}
