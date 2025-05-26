using System.Text;
using Confluent.Kafka;
using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;
using Headers = Confluent.Kafka.Headers;
using Constants = Verx.Enterprise.Correlation.Constants;

namespace Verx.Enterprise.MessageBroker.RabbitMQ.Extensions;

[ExcludeFromCodeCoverage]
internal static class CorrelationExtensions
{
    internal static Guid CorrelationId(this Headers headers)
    {
        if (headers.FirstOrDefault(h => h.Key.Equals(Constants.CorrelationIdHeader, StringComparison.OrdinalIgnoreCase)) is not IHeader header)
            return Guid.CreateVersion7();

        var value = Encoding.UTF8.GetString(header.GetValueBytes());
        if (Guid.TryParse(value, out var guid))
            return guid;

        return Guid.CreateVersion7();
    }

    internal static Guid CorrelationId(this IBasicProperties basicProperties)
    {
        try
        {
            object? value = new();
            if (basicProperties?.Headers.TryGetValue(Correlation.Constants.CorrelationIdHeader, out value) ?? true && value is byte[])
            {
                var str = Encoding.UTF8.GetString((byte[])value);
                if (Guid.TryParse(str, out var guid))
                    return guid;
            }
        }
        catch { }

        return Guid.CreateVersion7();
    }
}
