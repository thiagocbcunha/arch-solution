using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Verx.Enterprise.Correlation;

[ExcludeFromCodeCoverage]
public static class Setup
{
    public static IServiceCollection TryAddCorrelation(this IServiceCollection service)
    {
        service.TryAddScoped<ApplicationCorrelation>();
        service.TryAddScoped<ICorrelation>(sp => sp.GetService<ApplicationCorrelation>() ?? new ApplicationCorrelation(sp));

        return service;
    }
}
