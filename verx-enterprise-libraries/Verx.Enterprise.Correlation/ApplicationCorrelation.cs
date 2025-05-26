using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Verx.Enterprise.Correlation;

[ExcludeFromCodeCoverage]
public class ApplicationCorrelation(IServiceProvider serviceProvider) : ICorrelation
{
    private Guid _correlationId = Guid.Parse(serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext?.Items[Constants.CorrelationIdHeader]?.ToString() ?? Guid.CreateVersion7().ToString());

    public Guid Id => _correlationId;

    public void SetCorrelation(Guid guid)
    {
        _correlationId = guid;
    }
}

