namespace Verx.TransactionFlow.Domain.Options;

public class ObservabilitySettings
{
    public string Version { get; set; } = null!;
    public required string OTELEndpoint { get; set; } = null!;
}
