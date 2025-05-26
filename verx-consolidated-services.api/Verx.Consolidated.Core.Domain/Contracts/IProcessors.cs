namespace Verx.Consolidated.Domain.Contracts;

public interface IProcessors<TEvent>
{
    Task<bool> ProcessingEvents(CancellationToken cancellationToken);
}
