using System.Text.Json.Serialization;

namespace Supervisor.Application.Common.Options;

public class RuntimeProcessProfile
{
    [JsonPropertyName("url")] public required string Url { get; set; }

    [JsonPropertyName("processId")] public required int ProcessId { get; set; }

    [JsonPropertyName("pipe-name")] public required string PipeName { get; set; }
}