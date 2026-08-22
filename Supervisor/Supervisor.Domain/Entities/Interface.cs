using System.Text.Json.Serialization;

namespace Supervisor.Domain.Entities;

public class Interface
{
    [JsonPropertyName("uniqueName")] public required string UniqueName { get; set; }

    [JsonPropertyName("name")] public required string Name { get; set; }

    [JsonPropertyName("requiredRuntimeVersion")]
    public required Version RequiredRuntimeVersion { get; set; }

    [JsonPropertyName("version")] public required Version Version { get; set; }
}