using Microsoft.AspNetCore.Builder;

namespace Verx.Authentication.Service.Crosscuting;

public interface IModuleInitializer
{
    void Initialize(WebApplicationBuilder builder);
}
