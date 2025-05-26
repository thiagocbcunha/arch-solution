using MongoDB.Bson;
using RabbitMQ.Client;
using MongoDB.Bson.Serialization;
using Verx.Consolidated.Infra.Mongo;
using Verx.Enterprise.MessageBroker;
using Verx.Consolidated.Domain.Options;
using Verx.Consolidated.Domain.Contracts;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Serializers;
using Microsoft.Extensions.DependencyInjection;
using Verx.Consolidated.Infra.Dapper.Connection;
using Verx.Consolidated.Infra.Dapper.Repositories;
using IConnectionFactory = Verx.Consolidated.Infra.Dapper.Contracts.IConnectionFactory;

namespace Verx.Consolidated.Infra;

public static class Setup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        services.AddRabbitMQ(configuration);
        services.AddScoped<IConnectionFactory, DapperConnectionFactory>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IConsolidatedNSqlRepository, ConsolidatedRepository>();

        return services;
    }

    private static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRabbit(sp =>
        {
            var rabbitSettings = configuration.GetSection(nameof(RabbitSettings)).Get<RabbitSettings>();

            ArgumentNullException.ThrowIfNull(rabbitSettings, "RabbitSettings cannot be null");

            return new ConnectionFactory
            {
                Port = rabbitSettings.Port,
                Uri = new Uri(rabbitSettings.Host),
                UserName = rabbitSettings.UserName,
                Password = rabbitSettings.Password,
                VirtualHost = rabbitSettings.VirtualHost
            };
        });

        return services;
    }
}