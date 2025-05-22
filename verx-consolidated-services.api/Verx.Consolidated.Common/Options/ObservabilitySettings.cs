namespace Verx.Consolidated.Common.Options;

public class ObservabilitySettings
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string OTELEndpoint { get; set; } = string.Empty;
    public string LogstashEndpoint { get; set; } = string.Empty;
}
