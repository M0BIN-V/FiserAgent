using System.Text.Json.Serialization;

namespace Supervisor.Application.Options;

public class RuntimeManifest
{
    [JsonPropertyName("version")] public required string Version { get; set; }
}