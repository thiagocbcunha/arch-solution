using System.Text;
using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;
using OpenTelemetry.Context.Propagation;
using Headers = Confluent.Kafka.Headers;

namespace Verx.Enterprise.MessageBroker.RabbitMQ.Extensions;

[ExcludeFromCodeCoverage]
internal static class PropagationContextExtensions
{
    internal static PropagationContext ExtractContext(this Headers headers)
    {
        try
        {
            var carrier = headers.ToDictionary(h => h.Key, h => Encoding.UTF8.GetString(h.GetValueBytes()));

            return Propagators.DefaultTextMapPropagator.Extract(default, carrier, (dict, key)
                => dict.TryGetValue(key, out var value) ? [value] : []);
        }
        catch
        {
            return default;
        }
    }
    internal static PropagationContext ExtractContext(this IBasicProperties headers)
    {
        try
        {
            return Propagators.DefaultTextMapPropagator.Extract(default, headers, (props, key) =>
            {
                if (props.Headers != null && props.Headers.TryGetValue(key, out var val))
                    return [Encoding.UTF8.GetString((byte[])val!)];
                return [];
            });
        }
        catch
        {
            return default;
        }
    }
}
