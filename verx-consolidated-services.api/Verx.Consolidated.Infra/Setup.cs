using MassTransit;
using Verx.Consolidated.Domain.Dtos;
using Verx.Consolidated.Infra.Mongo;
using Verx.Consolidated.Infra.RabbitMQ;
using Verx.Consolidated.Domain.Contracts;
using Verx.Consolidated.Application.Consumers;
using Microsoft.Extensions.Configuration;
using Verx.Consolidated.Infra.Dapper.Contracts;
using Verx.Consolidated.Infra.RabbitMQ.Options;
using Verx.Consolidated.Infra.Dapper.Connection;
using Verx.Consolidated.Infra.Dapper.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Verx.Consolidated.Common.Logging;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace Verx.Consolidated.Infra;

public static class Setup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        services.AddRabbitMQ(configuration);
        services.AddScoped<IMessagingSender, MessageSender>();
        services.AddScoped<ICorrelation, HttpContextVerxCorrelation>();
        services.AddScoped<IConnectionFactory, DapperConnectionFactory>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IConsolidatedNSqlRepository, ConsolidatedRepository>();

        return services;
    }

    private static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitConfiguration = new RabbitSettings();
        configuration.GetRequiredSection(nameof(RabbitSettings)).Bind(rabbitConfiguration);


        services.AddMassTransit(m =>
        {
            m.AddConsumer<ConsolidatedConsumer>();

            m.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(rabbitConfiguration.Host, rabbitConfiguration.Port, rabbitConfiguration.VirtualHost, c =>
                {
                    c.Username(rabbitConfiguration.UserName);
                    c.Password(rabbitConfiguration.Password);
                });

                cfg.Durable = false;

                cfg.ConfigureEndpoints(ctx);

                Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Message<ConsolidatedDto>(x => x.SetEntityName("VerxConsolidated.ConsolidatedDto.Event"));
                });
            });
        });

        return services;
    }
}