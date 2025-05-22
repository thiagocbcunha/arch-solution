using Verx.Consolidated.Domain;
using Verx.Consolidated.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Verx.Consolidated.Infra.IoC;

public static class Setup
{
    public static IServiceCollection SetupApplication(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddDomain();
        service.AddApplication();
        service.AddInfrastructure(configuration);

        return service;
    }
}