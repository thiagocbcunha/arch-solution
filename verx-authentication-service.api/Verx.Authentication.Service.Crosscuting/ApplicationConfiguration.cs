using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Verx.Authentication.Service.Infrastructure;

namespace Verx.Authentication.Service.Crosscuting;

public static class ApplicationConfiguration
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfra(configuration);

        return services;
    }
}