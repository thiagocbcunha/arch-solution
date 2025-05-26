namespace Verx.TransactionFlow.Domain.Options;

/// <summary>
/// Kafka settings.
/// </summary>
public record KafkaSettings
{
    /// <summary>
    /// Topic name. 
    /// </summary>
    public required string Topic { get; set; }

    /// <summary>
    /// Bootstrap servers.
    /// </summary>
    public required string BootstrapServers { get; set; }

    /// <summary>
    /// Consumer group.
    /// </summary>
    public required string ConsumerGroup { get; set; }
}
