using RabbitMQ.Client;
using Confluent.Kafka;
using System.Threading.Channels;
using Verx.Enterprise.Correlation;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Verx.Enterprise.MessageBroker.Kafka.Extensions;
using Verx.Enterprise.MessageBroker.RabbitMQ.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Verx.Enterprise.MessageBroker;

[ExcludeFromCodeCoverage]
public static class Setup
{
    private static IServiceCollection AddChannel<TMessage>(this IServiceCollection services)
        where TMessage : class, new()
    {
        var channeOptions = new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true,
            AllowSynchronousContinuations = true
        };

        services.TryAddScoped<IChannel<TMessage>, BrokerChannel<TMessage>>();
        services.TryAddSingleton(Channel.CreateUnbounded<EventPackage<TMessage>>(channeOptions));

        return services;
    }

    private static IServiceCollection AddInternalTrace(this IServiceCollection services)
    {
        services.TryAddScoped<InternalTraceFacade>();

        return services;
    }

    public static IServiceCollection AddRabbit(this IServiceCollection services, Func<IServiceProvider, ConnectionFactory> connectionFactory)
    {
        services.AddInternalTrace();
        services.TryAddCorrelation();
        services.AddRabbitInternal(connectionFactory);

        return services;
    }

    public static IServiceCollection AddKafkaProducer(this IServiceCollection services, Func<IServiceProvider, ProducerConfig> connectionFactory)
    {
        services.AddInternalTrace();
        services.TryAddCorrelation();
        services.AddKafkaProducerInternal(connectionFactory);

        return services;
    }

    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, Func<IServiceProvider, ConsumerConfig> connectionFactory)
    {
        services.AddInternalTrace();
        services.TryAddCorrelation();
        services.AddKafkaConsumerInternal(connectionFactory);

        return services;
    }

    public static IServiceCollection AddConsumerFor<TMessage>(this IServiceCollection services, BrokerType brokerType)
        where TMessage : class, new()
    {
        services.AddChannel<TMessage>();

        if (brokerType == BrokerType.Rabbit || brokerType == BrokerType.Both)
            services.AddRabbitConsumerFor<TMessage>();

        if (brokerType == BrokerType.Kafka || brokerType == BrokerType.Both)
            services.AddKafkaConsumerFor<TMessage>();

        return services;
    }
}
