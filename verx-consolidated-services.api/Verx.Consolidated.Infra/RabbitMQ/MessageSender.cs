using MassTransit;
using Verx.Consolidated.Domain.Contracts;

namespace Verx.Consolidated.Infra.RabbitMQ;

public class MessageSender(IBus _bus) : IMessagingSender
{
    public async Task Send<TMessage>(TMessage message) where TMessage : class
    {
        var sendEndpoint = await _bus.GetSendEndpoint(new Uri($"queue:VerxConsolidated.{typeof(TMessage).Name}.Event"));
        await sendEndpoint.Send(message);
    }
}