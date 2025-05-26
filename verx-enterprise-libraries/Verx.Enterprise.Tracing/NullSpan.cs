using System.Diagnostics;

namespace Verx.Enterprise.Tracing;

/// <summary>
/// A no-op implementation of <see cref="ISpan"/> that performs no tracing operations.
/// This class is used as a placeholder when tracing is disabled or not required.
/// All methods are implemented as no-operations and do not record any data.
/// </summary>
public class NullSpan : ISpan, IDisposable
{
    /// <inheritdoc/>
    public ActivityContext Context => default;

    /// <inheritdoc/>
    public ISpan SetKey(string key, object value) => this;

    /// <inheritdoc/>
    public ISpan NewMessage(string message) => this;

    /// <inheritdoc/>
    public void Success() { }

    /// <inheritdoc/>
    public void Failure(string? description = null) { }

    /// <inheritdoc/>
    public void Dispose()
    { }
}
