using Microsoft.AspNetCore.Http;

namespace Verx.TransactionFlow.Common.Logging;

/// <summary>
/// VerxCorrelation is used to get the correlation id from the http context.
/// </summary>
/// <param name="httpContextAccessor"></param>
public class HttpContextVerxCorrelation(IHttpContextAccessor httpContextAccessor) : ICorrelation
{
    private Guid _correlationId = Guid.Parse(httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString());

    /// <summary>
    /// Gets the correlation id from the http context.
    /// </summary>
    public Guid Id => _correlationId;
}
