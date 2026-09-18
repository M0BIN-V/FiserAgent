namespace Runtime.Application.Models;

public sealed class McpServerConfiguration
{
    public string? Type { get; set; }

    public string? Url { get; set; }

    public string? Command { get; set; }

    public List<string> Args { get; set; } = [];

    public Dictionary<string, string> Headers { get; set; } = [];

    public Dictionary<string, string> Env { get; set; } = [];
}