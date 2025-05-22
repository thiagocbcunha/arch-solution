using MassTransit;

namespace Verx.TransactionFlow.Infrastructure.MessageBrokers;


[Serializable]
public class EventMessageNotAckExceptionException : Exception
{
    public ConsumeContext ConsumeContext { get; init; }

    public EventMessageNotAckExceptionException(ConsumeContext consumeContext) 
	{
		ConsumeContext = consumeContext;
    }
	public EventMessageNotAckExceptionException(string message) : base(message) { }
	public EventMessageNotAckExceptionException(string message, Exception inner) : base(message, inner) { }
	protected EventMessageNotAckExceptionException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}