using Verx.Enterprise.Tracing;

namespace Verx.Enterprise.MessageBroker;

public interface IChannel<TMessage>
    where TMessage : class, new()
{
    IAsyncEnumerable<TMessage?> ConsumeAsync(CancellationToken cancellationToken = default);
    Task RaiserError(ISpan span);
}
