using Verx.TransactionFlow.Common.Logging;

namespace Verx.TransactionFlow.Infrastructure.MessageBrokers;

/// <summary>
/// Represents a correlation mechanism for message processing.
/// </summary>
public class EventMessageCorrelation : ICorrelation
{
    private Guid _correlationId;

    /// <inheritdoc />
    public Guid Id => _correlationId;

    /// <summary>
    /// Sets the correlation ID for the current instance.
    /// </summary>
    /// <param name="guid"></param>
    public void SetCorrelation(Guid guid)
    {
        _correlationId = guid;
    }
}

