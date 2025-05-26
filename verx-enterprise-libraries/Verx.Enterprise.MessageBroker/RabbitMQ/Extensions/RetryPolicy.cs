using System.Text;
using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;

namespace Verx.Enterprise.MessageBroker.RabbitMQ.Extensions;

[ExcludeFromCodeCoverage]
internal static class RetryPolicy
{
    internal static long CalculateDalay(this int retryCount, long timeMillisecond) => (long)Math.Pow(2, retryCount) * timeMillisecond;

    internal static int GetRetryCount(this IBasicProperties props)
    {
        if (props.Headers != null && props.Headers.TryGetValue("x-retry-count", out var raw))
        {
            var bytes = raw as byte[];
            var str = Encoding.UTF8.GetString(bytes!);
            return int.TryParse(str, out var parsed) ? parsed : 0;
        }
        return 0;
    }

    internal static Guid GetCorrelationId(this IBasicProperties props)
    {
        if (props.Headers != null && props.Headers.TryGetValue("x-correlation-id", out var raw))
        {
            var bytes = raw as byte[];
            var str = Encoding.UTF8.GetString(bytes!);
            return Guid.TryParse(str, out var parsed) ? parsed : Guid.CreateVersion7();
        }

        return Guid.CreateVersion7();
    }

    internal static void AddRetryHeader(this IBasicProperties props, int retryCount)
    {
        props.Headers ??= new Dictionary<string, object>();
        props.Headers["x-retry-count"] = Encoding.UTF8.GetBytes(retryCount.ToString());
    }
}
