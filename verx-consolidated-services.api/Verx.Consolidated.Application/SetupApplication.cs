using MediatR;
using RabbitMQ.Client;
using FluentValidation;
using Verx.Enterprise.MessageBroker;
using Verx.Consolidated.Domain.Options;
using Verx.Enterprise.Common.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Verx.Consolidated.Application;

public static class SetupApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.AddRabbit(sp =>
        {
            var rabbitSettings = configuration.GetSection(nameof(RabbitSettings)).Get<RabbitSettings>();

            ArgumentNullException.ThrowIfNull(rabbitSettings, "RabbitSettings cannot be null");

            if (!Uri.TryCreate(rabbitSettings.Host, UriKind.Absolute, out Uri uri))
                throw new ArgumentException("Invalid RabbitMQ host URI", nameof(rabbitSettings.Host));

            return new ConnectionFactory
            {
                Uri = uri,
                Port = rabbitSettings.Port,
                UserName = rabbitSettings.UserName,
                Password = rabbitSettings.Password,
                VirtualHost = rabbitSettings.VirtualHost
            };
        });

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
