using System.Text.Json.Serialization;

namespace Fiser.Supervisor.Options;

public class RuntimeManifest
{
    [JsonPropertyName("version")] public required string Version { get; set; }
}