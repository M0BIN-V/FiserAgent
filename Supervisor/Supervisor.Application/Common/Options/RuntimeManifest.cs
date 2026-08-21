using System.Text.Json.Serialization;

namespace Supervisor.Application.Common.Options;

public class RuntimeManifest
{
    [JsonPropertyName("version")] public required string Version { get; set; }
}