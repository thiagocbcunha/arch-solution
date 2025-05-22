using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Verx.Authentication.Service.Domain.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Verx.Authentication.Service.Infrastructure.Data;
using Verx.Autentication.Service.Domain.Entities;

namespace Verx.Authentication.Service.Infrastructure;

/// <summary>
/// 
/// </summary>
public static class InfraSetup
{
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddContexts(configuration)
            .AddJwtToken(configuration);
    }
    #region PRIVATE METHODS
    private static IServiceCollection AddContexts(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services//.AddIdentity<IdentityUser, IdentityRole>()
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddIdentityServer()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.EnableTokenCleanup = true;
            });

        return services;
    }

    private static IServiceCollection AddJwtToken(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfiguration = configuration.GetSection(nameof(JwtConfiguration)).Get<JwtConfiguration>();

        ArgumentNullException.ThrowIfNull(jwtConfiguration);

        services.Configure<JwtConfiguration>(configuration.GetSection(nameof(JwtConfiguration)));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfiguration.Issuer,
                ValidAudience = jwtConfiguration.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Key))
            };
        });

        return services;
    }
    #endregion
}