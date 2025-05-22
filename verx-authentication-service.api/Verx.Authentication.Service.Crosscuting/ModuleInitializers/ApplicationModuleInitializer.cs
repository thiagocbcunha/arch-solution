using Microsoft.AspNetCore.Builder;
using Verx.Authentication.Service.Application;

namespace Verx.Authentication.Service.Crosscuting.ModuleInitializers;

public class ApplicationModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddApplication(builder.Configuration);
    }
}