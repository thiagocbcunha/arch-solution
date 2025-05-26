using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;

namespace Verx.Enterprise.MessageBroker.RabbitMQ;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ExchangeAttribute(string nameExchange) : Attribute
{
    private readonly string? _exchangeType;
    private readonly string _nameExchange = ValidateAndReturnExchangeName(nameExchange);

    private static string ValidateAndReturnExchangeName(string nameExchange)
    {
        ThrowIfInvalidExchangeName(nameExchange);
        return nameExchange;
    }

    public ExchangeAttribute(string nameExchange, string? exchangeType)
        : this(nameExchange)
    {
        if (!string.IsNullOrWhiteSpace(exchangeType))
            ThrowIfInvalidExchangeType(exchangeType);

        _exchangeType = exchangeType;
    }

    public string ExchangeName => _nameExchange;

    public string? TypeExchange => _exchangeType ?? ExchangeType.Direct;

    private static void ThrowIfInvalidExchangeType(string exchangeType)
    {
        if (string.IsNullOrWhiteSpace(exchangeType))
            throw new ArgumentException("Exchange type cannot be null or empty.", nameof(exchangeType));

        if (!ExchangeType.All().Contains(exchangeType))
            throw new ArgumentException($"Invalid exchange type: {exchangeType}.", nameof(exchangeType));
    }

    private static void ThrowIfInvalidExchangeName(string nameExchange)
    {
        if (string.IsNullOrWhiteSpace(nameExchange))
            throw new ArgumentException("Exchange name cannot be null or empty.", nameof(nameExchange));

        if (nameExchange.Length > 255)
            throw new ArgumentException("Exchange name must be 255 characters or fewer.", nameof(nameExchange));

        if (nameExchange.Contains('\0'))
            throw new ArgumentException("Exchange name cannot contain null characters.", nameof(nameExchange));

        if (nameExchange.StartsWith("amq."))
            throw new ArgumentException("Exchange names starting with 'amq.' are reserved for internal use.", nameof(nameExchange));
    }
}
