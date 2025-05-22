namespace Verx.Consolidated.Infra.RabbitMQ.Options;

/// <summary>
/// Represents the configuration settings required to connect and interact with a RabbitMQ server.
/// </summary>
/// <remarks>
/// This record encapsulates all necessary properties for configuring a RabbitMQ connection, including
/// queue name, endpoint, authentication credentials, retry policy, and durability settings.
/// </remarks>
public record RabbitSettings
{
    /// <summary>
    /// Gets or sets the RabbitMQ server host name or IP address.
    /// </summary>
    public string Host { get; set; } = null!;

    /// <summary>
    /// Gets or sets the username used to authenticate with RabbitMQ.
    /// </summary>
    public string UserName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the password used to authenticate with RabbitMQ.
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts for RabbitMQ operations.
    /// </summary>
    public int MaxRetry { get; set; } = 3;

    /// <summary>
    /// Gets or sets a value indicating whether the queue should be durable (persisted to disk).
    /// </summary>
    public bool Durable { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the queue should be exclusive (only accessible by the current connection).
    /// </summary>
    public ushort Port { get; set; }

    /// <summary>
    /// Gets or sets the virtual host to use when connecting to RabbitMQ.
    /// </summary>
    public string VirtualHost { get; set; } = "/";
}
