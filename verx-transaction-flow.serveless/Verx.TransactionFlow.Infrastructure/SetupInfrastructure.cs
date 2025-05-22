using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Verx.TransactionFlow.Common.Logging;
using Verx.TransactionFlow.Domain.Options;
using Verx.TransactionFlow.Domain.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Verx.TransactionFlow.Infrastructure.MessageBrokers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Verx.TransactionFlow.Infrastructure.MessageBrokers.Kafka;

namespace Verx.TransactionFlow.Infrastructure;

/// <summary>
/// Provides extension methods for setting up infrastructure services, including message broker consumers and producers.
/// </summary>
public static class SetupInfrastructure
{
    /// <summary>
    /// Registers infrastructure services, including Kafka and RabbitMQ producers, and configures related settings.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the infrastructure services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance containing infrastructure settings.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaSettings>(configuration.GetSection(nameof(KafkaSettings)));
        services.Configure<ConsolidatedSettings>(configuration.GetSection(nameof(ConsolidatedSettings)));

        return services;
    }

    /// <summary>
    /// Creates and adds a channel for the specified <typeparamref name="TMessage"/> type to the dependency injection container.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    private static IServiceCollection AddChannel<TMessage>(this IServiceCollection services)
        where TMessage : class
    {
        var channel = Channel.CreateUnbounded<EventMessage<TMessage>>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true,
            AllowSynchronousContinuations = true
        });

        services.TryAddSingleton(channel);

        return services;
    }

    /// <summary>
    /// Adds a Kafka consumer for the specified <typeparamref name="TMessage"/> type to the dependency injection container.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddKafkaProducerBy<TMessage>(this IServiceCollection services)
        where TMessage : class
    {
        services.AddChannel<TMessage>();
        services.AddScoped<IKafkaProducer<TMessage>, KafkaProducer<TMessage>>();

        return services;
    }

    /// <summary>
    /// Adds a Kafka consumer for the specified <typeparamref name="TMessage"/> type to the dependency injection container.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddKafkaConsumerBy<TMessage>(this IServiceCollection services)
        where TMessage : class
    {
        services.AddChannel<TMessage>();
        services.AddHostedService<KafkaConsumer<TMessage>>();

        return services;
    }

    /// <summary>
    /// Adds HTTP header-based correlation services to the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the infrastructure services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddHttpHeaderCorrelation(this IServiceCollection services)
    {
        services.AddScoped<ICorrelation, HttpContextVerxCorrelation>();

        return services;
    }

    /// <summary>
    /// Adds event message correlation services to the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the infrastructure services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddEventMessageCorrelation(this IServiceCollection services)
    {
        services.AddScoped<EventMessageCorrelation>();
        services.AddScoped<ICorrelation>(op => op.GetRequiredService<EventMessageCorrelation>());

        return services;
    }
}
