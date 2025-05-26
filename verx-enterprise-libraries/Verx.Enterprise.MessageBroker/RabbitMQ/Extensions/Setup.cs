using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Verx.Enterprise.MessageBroker.RabbitMQ.Client;

namespace Verx.Enterprise.MessageBroker.RabbitMQ.Extensions;

[ExcludeFromCodeCoverage]
internal static class Setup
{
    internal static IServiceCollection AddRabbitInternal(this IServiceCollection services, Func<IServiceProvider, ConnectionFactory> connectionFactory)
    {
        services.AddSingleton(sp => connectionFactory(sp).CreateConnection());
        services.AddScoped(typeof(IRabbitProducer<>), typeof(RabbitProducer<>));

        return services;
    }

    internal static IServiceCollection AddRabbitConsumerFor<TMessage>(this IServiceCollection services)
        where TMessage : class, new()
    {
        services.AddHostedService<RabbitConsumer<TMessage>>();

        return services;
    }
}
