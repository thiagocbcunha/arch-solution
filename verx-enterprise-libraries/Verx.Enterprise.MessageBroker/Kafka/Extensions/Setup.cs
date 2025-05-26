using Confluent.Kafka;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Verx.Enterprise.MessageBroker.Kafka.Client;

namespace Verx.Enterprise.MessageBroker.Kafka.Extensions;

[ExcludeFromCodeCoverage]
internal static class Setup
{
    internal static IServiceCollection AddKafkaProducerInternal(this IServiceCollection services, Func<IServiceProvider, ProducerConfig> connectionFactory)
    {
        services.AddSingleton(sp => connectionFactory(sp));
        services.AddScoped(typeof(IKafkaProducer<>), typeof(KafkaProducer<>));

        return services;
    }

    internal static IServiceCollection AddKafkaConsumerInternal(this IServiceCollection services, Func<IServiceProvider, ConsumerConfig> connectionFactory)
    {
        services.AddSingleton(sp => connectionFactory(sp));

        return services;
    }

    internal static IServiceCollection AddKafkaConsumerFor<TMessage>(this IServiceCollection services)
        where TMessage : class, new()
    {
        services.AddHostedService<KafkaConsumer<TMessage>>();

        return services;
    }
}
