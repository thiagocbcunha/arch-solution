using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using System.Text.Json.Serialization;
using Verx.Authentication.Service.Application;
using Microsoft.Extensions.DependencyInjection;
using Verx.Authentication.Service.Common.HealthChecks;

namespace Verx.Authentication.Service.Crosscuting.ModuleInitializers;

public class WebApiModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApplicationJsonSerializaerContext.Default);
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        builder.AddBasicHealthChecks();
    }
}
