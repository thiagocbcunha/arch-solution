using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Duende.IdentityServer.EntityFramework.DbContexts;

namespace Verx.Authentication.Service.Infrastructure.Data;

public static class ConfigureMigration
{
    public static IHost ExecuteMigration(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        applicationDbContext.Database.Migrate();
        
        var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        configurationDbContext.Database.Migrate();

        var persistedGrantDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
        persistedGrantDbContext.Database.Migrate();

        return host;
    }
}
