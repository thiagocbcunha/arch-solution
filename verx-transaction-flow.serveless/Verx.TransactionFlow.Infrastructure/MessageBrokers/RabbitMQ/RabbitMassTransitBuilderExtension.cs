using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Verx.TransactionFlow.Infrastructure.MessageBrokers.RabbitMQ;

[ExcludeFromCodeCoverage]
public static class RabbitMassTransitBuilderExtension
{
    public static RabbitMassTransitBuilder RabbitBuilder(this IServiceCollection service, IConfiguration configuration)
        => new(service, configuration);
}
