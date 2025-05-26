using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

namespace Verx.Enterprise.WebApplications;

[ExcludeFromCodeCoverage]
public static class ConfigureWebApplicationExtension
{
    public static IApplicationBuilder ConfigureWebApplication(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceptionMiddleware>();
        return builder;
    }
}