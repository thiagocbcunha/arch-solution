using Microsoft.AspNetCore.Builder;
using Verx.Authentication.Service.Infrastructure;

namespace Verx.Authentication.Service.Crosscuting.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddInfra(builder.Configuration);
        //builder.Services.AddRedis(builder.Configuration);
    }
}