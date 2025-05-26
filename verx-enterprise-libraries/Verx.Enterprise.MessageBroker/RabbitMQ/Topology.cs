using RabbitMQ.Client;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace Verx.Enterprise.MessageBroker.RabbitMQ;

[ExcludeFromCodeCoverage]
internal class Topology
{
    public Topology(Type type)
    {
        var typeName = type.Name.ToLower();

        var exchange = $"{typeName}_exchange";
        if (type.GetCustomAttribute<ExchangeAttribute>(true) is ExchangeAttribute exchangeAttribute)
        {
            exchange = exchangeAttribute.ExchangeName;
            if (!string.IsNullOrWhiteSpace(exchangeAttribute.TypeExchange))
                ExchangeMainType = exchangeAttribute.TypeExchange;
        }

        Exchange = exchange;
        Queue = $"{typeName}_queue";

        DlqQueue = $"{Queue}_dlq";
        DlqExchange = $"{typeName}_dlq_exchange";
        
        RetryQueue = $"{Queue}_retry";
        RetryExchange = $"{typeName}_retry_exchange";
    }

    internal string Queue { get; init; }
    internal string Exchange { get; init; }
    internal string DlqQueue { get; init; }
    internal string DlqExchange { get; init; }
    internal string RetryQueue { get; init; }
    internal string RetryExchange { get; init; }
    internal string ExchangeMainType { get; init; } = ExchangeType.Direct;
}