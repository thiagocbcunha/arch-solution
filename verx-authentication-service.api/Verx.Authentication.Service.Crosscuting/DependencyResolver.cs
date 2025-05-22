using Microsoft.AspNetCore.Builder;
using Verx.Authentication.Service.Crosscuting.ModuleInitializers;

namespace Verx.Authentication.Service.Crosscuting;

public static class DependencyResolver
{
    public static void RegisterDependencies(this WebApplicationBuilder builder)
    {
        new WebApiModuleInitializer().Initialize(builder);
        new ApplicationModuleInitializer().Initialize(builder);
        new InfrastructureModuleInitializer().Initialize(builder);
    }
}