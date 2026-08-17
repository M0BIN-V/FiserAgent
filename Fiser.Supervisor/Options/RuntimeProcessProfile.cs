using System.Text.Json.Serialization;

namespace Fiser.Supervisor.Options;

public class RuntimeProcessProfile
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }
    
    [JsonPropertyName("processId")]
    public required int ProcessId { get; set; }
}