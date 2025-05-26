using RabbitMQ.Client;
using Verx.Enterprise.MessageBroker.RabbitMQ;

namespace Verx.TransactionFlow.Domain.Event;

[Exchange("transationcreated_exchange", ExchangeType.Fanout)]
[RetryPolicy(5, 10000, exponecial: true)]
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