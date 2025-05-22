using MassTransit;
using System.Diagnostics;
using System.Threading.Channels;
using Verx.TransactionFlow.Common.Contracts;

namespace Verx.TransactionFlow.Infrastructure.MessageBrokers.RabbitMQ;

/// <summary>
/// Represents a generic RabbitMQ consumer for processing messages of type <typeparamref name="TMessage"/>.
/// </summary>
/// <typeparam name="TMessage">The type of the message payload being consumed.</typeparam>
/// <remarks>
/// <para>
/// This consumer is designed to work with MassTransit and processes messages wrapped in <see cref="EventMessage{TMessage}"/>.
/// Upon receiving a message, it starts an activity for tracing, sets the correlation ID for distributed tracing,
/// logs the event, and writes the message to a provided channel for further processing.
/// </para>
/// <para>
/// If the message indicates success (<c>Success == true</c>), the activity is marked as successful.
/// Otherwise, the activity is marked as failed, a log entry is created, and an <see cref="EventMessageNotAckExceptionException"/> is thrown
/// to indicate that the message was not acknowledged.
/// </para>
/// <para>
/// This class is intended to be used as a dependency-injected consumer in a message-driven architecture.
/// </para>
/// </remarks>
/// <example>
/// Example usage in MassTransit configuration:
/// <code>
/// services.AddMassTransit(x =>
/// {
///     x.AddConsumer&lt;RabbitConsumer&lt;MyMessage&gt;&gt;();
///     // ... other configuration
/// });
/// </code>
/// </example>
public class RabbitConsumer<TMessage>(IActivityTracing activityTracing, Channel<EventMessage<TMessage>> channel, EventMessageCorrelation messageCorrelation) : IConsumer<EventMessage<TMessage>>
    where TMessage : class
{
    /// <summary>
    /// Consumes an <see cref="EventMessage{TMessage}"/> from RabbitMQ, processes it, and writes it to the provided channel.
    /// </summary>
    /// <param name="context">The context containing the message and metadata.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="EventMessageNotAckExceptionException">
    /// Thrown if the message is not acknowledged (i.e., <c>Success == false</c>).
    /// </exception>
    public async Task Consume(ConsumeContext<EventMessage<TMessage>> context)
    {
        using var activity = activityTracing.Start<RabbitConsumer<TMessage>>(ActivityKind.Consumer);
        messageCorrelation.SetCorrelation(context.Message.CorrelationId);
        activity.LogMessage($"Received message: {context.Message.EventName}");

        channel.Writer.WriteAsync(context.Message).AsTask().Wait();

        if (context.Message.Success)
        {
            activity.Success();
            return;
        }

        activity.LogMessage($"Message {context.Message.CorrelationId} not acknowledged.");
        activity.Failure("Message not acknowledged.");

        throw new EventMessageNotAckExceptionException(context);
    }
}
