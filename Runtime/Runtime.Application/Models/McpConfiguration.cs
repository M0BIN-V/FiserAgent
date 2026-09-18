namespace Runtime.Application.Models;

public sealed class McpConfiguration
{
    public Dictionary<string, McpServerConfiguration> McpServers { get; set; } = [];
}