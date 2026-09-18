using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using Runtime.Application.Models;

namespace Runtime.Application.Common.Services.Mcp;

public class McpTransportFactory(ILogger<McpTransportFactory> logger)
{
    public IClientTransport Create(string name, McpServerConfiguration configuration)
    {
        logger.LogInformation("Creating transport for {name} mcp", name);
        logger.LogInformation("MCP server '{name}' is using {type}.", name, configuration.Type);

        var type = configuration.Type?.Trim().ToLowerInvariant();

        return type switch
        {
            "http" => CreateHttp(name, configuration),

            "stdio" => CreateStdio(name, configuration),

            _ => throw new InvalidOperationException(
                $"MCP server '{name}' has unsupported transport type '{configuration.Type}'.")
        };
    }

    private HttpClientTransport CreateHttp(string name, McpServerConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.Url))
            throw new InvalidOperationException(
                $"MCP server '{name}' is using HTTP transport but no URL was specified.");

        var url = EnvironmentVariableResolver.Resolve(configuration.Url);

        var headers = EnvironmentVariableResolver.Resolve(configuration.Headers);

        return new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri(url),
            TransportMode = HttpTransportMode.StreamableHttp,
            AdditionalHeaders = headers
        });
    }

    private StdioClientTransport CreateStdio(string name, McpServerConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.Command))
            throw new InvalidOperationException(
                $"MCP server '{name}' is using stdio transport but no command was specified.");

        var command = EnvironmentVariableResolver.Resolve(configuration.Command);

        var arguments = configuration.Args
            .Select(EnvironmentVariableResolver.Resolve)
            .ToList();

        var environmentVariables = EnvironmentVariableResolver.Resolve(configuration.Env);

        return new StdioClientTransport(new StdioClientTransportOptions
        {
            Name = name,
            Command = command,
            Arguments = arguments,
            EnvironmentVariables = environmentVariables
        });
    }
}