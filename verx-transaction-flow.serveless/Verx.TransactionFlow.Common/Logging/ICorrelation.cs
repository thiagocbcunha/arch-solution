namespace Verx.TransactionFlow.Common.Logging;

/// <summary>
/// Represents a correlation context for tracking and associating related operations or requests.
/// </summary>
/// <remarks>
/// The <see cref="ICorrelation"/> interface is typically used to propagate a unique identifier
/// (correlation ID) throughout the lifecycle of a transaction, request, or workflow. This enables
/// consistent logging, tracing, and diagnostics across distributed systems or asynchronous flows.
/// </remarks>
public interface ICorrelation
{
    /// <summary>
    /// Gets the unique identifier associated with the current correlation context.
    /// </summary>
    /// <value>
    /// A <see cref="Guid"/> that uniquely identifies the correlation instance.
    /// </value>
    Guid Id { get; }
}
