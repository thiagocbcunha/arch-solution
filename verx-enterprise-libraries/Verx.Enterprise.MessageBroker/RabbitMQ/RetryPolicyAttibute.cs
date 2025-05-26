using System.Diagnostics.CodeAnalysis;

namespace Verx.Enterprise.MessageBroker.RabbitMQ;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class RetryPolicyAttribute(int maxRetry, int delay, bool exponecial = true) : Attribute
{
    public int MaxRetry { get; } = maxRetry;

    public int Delay { get; } = delay;

    public bool Exponential { get; } = exponecial;
}
