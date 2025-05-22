using Microsoft.Extensions.DependencyInjection;

namespace Verx.Consolidated.Domain;

public static class Setup
{
    public static IServiceCollection AddDomain(this IServiceCollection service)
    {
        return service;
    }
}