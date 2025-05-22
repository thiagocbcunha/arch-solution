using MediatR;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Verx.Authentication.Service.Domain;
using Microsoft.Extensions.DependencyInjection;
using Verx.Authentication.Service.Common.Validation;

namespace Verx.Authentication.Service.Application;

public static class SetupApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.AddDomain();
        services.AddValidators();
        services.AddValidatorsFromAssemblies(assemblies);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        var assembly = typeof(SetupApplication).Assembly;

        var validators = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces(), (type, interfaceType) => new { type, interfaceType })
            .Where(x => x.interfaceType.IsGenericType && x.interfaceType.GetGenericTypeDefinition() == typeof(IValidator<>))
            .ToList();

        foreach (var validator in validators)
            services.AddScoped(validator.interfaceType, validator.type);

        return services;
    }
}
