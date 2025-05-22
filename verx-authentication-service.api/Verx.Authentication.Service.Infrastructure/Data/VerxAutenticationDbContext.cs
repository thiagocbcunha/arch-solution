using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Design;
using Verx.Autentication.Service.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Verx.Authentication.Service.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
}

public class MigrationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

        builder.UseNpgsql(
               connectionString,
               b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
        );

        return new ApplicationDbContext(builder.Options);
    }
}
