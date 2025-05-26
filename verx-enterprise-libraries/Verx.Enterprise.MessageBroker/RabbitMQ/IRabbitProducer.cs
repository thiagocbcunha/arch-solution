using RabbitMQ.Client.Events;
using Verx.Enterprise.Tracing;

namespace Verx.Enterprise.MessageBroker.RabbitMQ;

public interface IRabbitProducer<in TMessage> : IProducer<TMessage>
    where TMessage : class
{
    Task PublishErroEventAsynt(ISpan span, BasicDeliverEventArgs eventMessage);
}
