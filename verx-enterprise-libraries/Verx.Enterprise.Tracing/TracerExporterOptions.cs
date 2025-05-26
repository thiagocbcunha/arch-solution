using OpenTelemetry.Exporter;
using System.Diagnostics.CodeAnalysis;

namespace Verx.Enterprise.Tracing;

[ExcludeFromCodeCoverage]
public class TracerExporterOptions : OtlpExporterOptions
{
    public string? Version { get; init; } = null;
    public required string ApplicationName { get; init; }
}