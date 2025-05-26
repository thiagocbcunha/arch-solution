using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;

namespace Verx.Enterprise.MessageBroker.RabbitMQ.Extensions;

[ExcludeFromCodeCoverage]
internal static class RabbitConfigurationExtension
{
    internal static void ConfigureTopoligy(this IModel channel, Topology topology)
    {
        channel.ExchangeDeclare(exchange: topology.Exchange, type: topology.ExchangeMainType, durable: true);

        channel.ExchangeDeclare(exchange: topology.DlqExchange, type: ExchangeType.Direct, durable: true);
        channel.QueueDeclare(topology.DlqQueue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(topology.DlqQueue, topology.DlqExchange, routingKey: topology.DlqQueue);

        channel.ExchangeDeclare(exchange: topology.RetryExchange, type: ExchangeType.Direct, durable: true);
        var retryArgs = new Dictionary<string, object>
        {
            { "x-message-ttl", 10000 },
            { "x-dead-letter-exchange", topology.Exchange },
            { "x-dead-letter-routing-key", topology.Queue },
        };
        channel.QueueDeclare(topology.RetryQueue, durable: true, exclusive: false, autoDelete: false, arguments: retryArgs);
        channel.QueueBind(topology.RetryQueue, topology.RetryExchange, routingKey: topology.RetryExchange);

        var mainArgs = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", topology.DlqExchange },
            { "x-dead-letter-routing-key", topology.DlqQueue }
        };

        channel.QueueDeclare(topology.Queue, durable: true, exclusive: false, autoDelete: false, arguments: mainArgs);
        channel.QueueBind(topology.Queue, topology.Exchange, routingKey: topology.Queue);
    }
}
