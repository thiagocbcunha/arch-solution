namespace Verx.TransactionFlow.Domain.Options;

/// <summary>
/// Kafka settings.
/// </summary>
public record KafkaSettings
{
    /// <summary>
    /// Topic name. 
    /// </summary>
    public string Topic { get; set; }

    /// <summary>
    /// Bootstrap servers.
    /// </summary>
    public string BootstrapServers { get; set; }

    /// <summary>
    /// Consumer group.
    /// </summary>
    public string ConsumerGroup { get; set; }
}
