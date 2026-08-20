using System.Text.Json.Serialization;

namespace Supervisor.Cli.Options;

public class RuntimeManifest
{
    [JsonPropertyName("version")] public required string Version { get; set; }
}