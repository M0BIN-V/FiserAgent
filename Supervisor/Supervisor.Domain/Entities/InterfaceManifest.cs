using System.Text.Json.Serialization;

namespace Supervisor.Domain.Entities;

public class InterfaceManifest
{
    [JsonPropertyName("unique-name")] public required string UniqueName { get; set; }

    [JsonPropertyName("name")] public required string Name { get; set; }

    [JsonPropertyName("minimum-runtime-version")]
    public required Version MinimumRuntimeVersion { get; set; }

    [JsonPropertyName("version")] public required Version Version { get; set; }
    
    [JsonPropertyName("binary-file-name")]
    public required string BinaryFileName { get; set; }
}