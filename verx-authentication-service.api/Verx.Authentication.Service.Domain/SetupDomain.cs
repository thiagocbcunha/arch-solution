using Microsoft.Extensions.DependencyInjection;
using Verx.Authentication.Service.Domain.Services;
using Verx.Authentication.Service.Domain.Contracts;

namespace Verx.Authentication.Service.Domain;

public static class SetupDomain
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services
            .AddScoped<ICredentialsService, CredentialsService>();
    }
}
