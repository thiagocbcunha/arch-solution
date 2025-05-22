using MassTransit;
using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Verx.TransactionFlow.Domain.Options;
using Verx.TransactionFlow.Domain.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json;

namespace Verx.TransactionFlow.Infrastructure.MessageBrokers.RabbitMQ;

/// <summary>
/// Provides a builder for configuring and registering MassTransit with RabbitMQ in a .NET application.
/// </summary>
/// <remarks>
/// This builder encapsulates the setup of MassTransit with RabbitMQ, including host configuration,
/// consumer registration, channel creation for message processing, and advanced endpoint options
/// such as retry and dead-letter queue bindings. It is designed to be used during application startup
/// to register all necessary MassTransit and RabbitMQ services into the dependency injection container.
/// </remarks>
public class RabbitMassTransitBuilder
{
    private readonly IServiceCollection _service;
    private readonly RabbitSettings _rabbitSettings;

    private Action<IBusRegistrationConfigurator> _massTransitAction = c => { };
    private Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> _usingRabbitMqAction = (c, b) => { };

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMassTransitBuilder"/> class.
    /// Binds RabbitMQ settings from configuration and prepares MassTransit registration.
    /// </summary>
    /// <param name="service">The service collection to register dependencies with.</param>
    /// <param name="configuration">The application configuration containing RabbitMQ settings.</param>
    public RabbitMassTransitBuilder(IServiceCollection service, IConfiguration configuration)
    {
        _service = service;
        _rabbitSettings = new RabbitSettings();
        configuration.GetRequiredSection(nameof(RabbitSettings)).Bind(_rabbitSettings);

        Console.WriteLine(JsonSerializer.Serialize(_rabbitSettings));

        _massTransitAction += c => c.UsingRabbitMq(_usingRabbitMqAction);

        _usingRabbitMqAction += (context, rabbitConfig) =>
        {
            rabbitConfig.Host(_rabbitSettings.Host, _rabbitSettings.Port, _rabbitSettings.VirtualHost, hostConfig =>
            {
                hostConfig.Username(_rabbitSettings.UserName);
                hostConfig.Password(_rabbitSettings.Password);
            });

            rabbitConfig.Durable = _rabbitSettings.Durable;

            rabbitConfig.ConfigureEndpoints(context);
        };
    }

    /// <summary>
    /// Adds a singleton unbounded channel for the specified message type to the service collection.
    /// This channel is used for internal message processing between consumers and other services.
    /// </summary>
    /// <typeparam name="TMessage">The type of message for the channel.</typeparam>
    private void AddChannel<TMessage>()
        where TMessage : class
    {
        var channel = Channel.CreateUnbounded<EventMessage<TMessage>>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true,
            AllowSynchronousContinuations = true
        });

        _service.TryAddSingleton(channel);
    }

    /// <summary>
    /// Registers a RabbitMQ producer service for sending messages to RabbitMQ.
    /// </summary>
    /// <returns></returns>
    public RabbitMassTransitBuilder AddRabbitProducer()
    {
        _service.AddScoped<IRabbitProducer, RabbitProducer>();

        return this;
    }

    /// <summary>
    /// Registers a RabbitMQ consumer for the specified message type, configures its receive endpoint,
    /// retry policy, dead-letter queue, and redelivery intervals. Also adds a processing channel for the message type.
    /// </summary>
    /// <typeparam name="TMessage">The type of message the consumer will handle.</typeparam>
    public RabbitMassTransitBuilder AddRabbitConsumer<TMessage>()
        where TMessage : class
    {
        string nameConsumer = typeof(TMessage).Name.ToLower();

        AddChannel<TMessage>();

        _massTransitAction += c =>
            c.AddConsumer<RabbitConsumer<TMessage>>();

        _usingRabbitMqAction += (context, rabbitConfig) =>
        {
            rabbitConfig.ReceiveEndpoint($"{nameConsumer}-queue", endpointConfig =>
            {
                endpointConfig.Bind($"{nameConsumer}-exchange", exchangeConfig =>
                {
                    exchangeConfig.ExchangeType = "fanout";
                });

                endpointConfig.UseMessageRetry(retryconfig =>
                    retryconfig.Exponential(
                        retryLimit: _rabbitSettings.MaxRetry,
                        minInterval: TimeSpan.FromSeconds(5),
                        maxInterval: TimeSpan.FromMinutes(1),
                        intervalDelta: TimeSpan.FromSeconds(10)
                    )
                );

                endpointConfig.BindDeadLetterQueue($"{nameConsumer}-dlq-exchange", $"{nameConsumer}-dlq");

                endpointConfig.UseScheduledRedelivery(redeliveryConfig =>
                    redeliveryConfig.Intervals(
                        TimeSpan.FromMinutes(1),
                        TimeSpan.FromMinutes(2),
                        TimeSpan.FromMinutes(5)
                    )
                );

                endpointConfig.ConfigureConsumer<RabbitConsumer<TMessage>>(context);
            });
        };

        return this;
    }

    /// <summary>
    /// Finalizes the MassTransit and RabbitMQ configuration and registers all services with the dependency injection container.
    /// Should be called after all hosts and consumers have been added.
    /// </summary>
    public void Build()
    {
        _service.AddMassTransit(_massTransitAction);
    }
}
