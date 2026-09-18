using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using Runtime.Application.Common.Abstractions;
using Runtime.Application.Models;

namespace Runtime.Application.Common.Services.Mcp;

public class McpToolProvider(
    McpTransportFactory transportFactory,
    ILogger<McpToolProvider> logger) : IMcpToolProvider, IAsyncDisposable
{
    private List<McpClient> _clients = [];

    public async ValueTask DisposeAsync()
    {
        foreach (var client in _clients)
        {
            logger.LogInformation("disposing mcp client :{client}", client);
            await client.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }

    public async Task<IReadOnlyList<AIFunction>> GetToolsAsync(CancellationToken ct = default)
    {
        var configurationPath = Path.Combine(AppContext.BaseDirectory, "mcp.json");

        logger.LogInformation("Reading mcp configurations from : {path}", configurationPath);

        if (!File.Exists(configurationPath))
            throw new FileNotFoundException("MCP configuration file was not found.", configurationPath);

        await using var stream = File.OpenRead(configurationPath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var configuration = await JsonSerializer.DeserializeAsync<McpConfiguration>(stream, options, ct);

        if (configuration is null)
            throw new InvalidOperationException($"Could not deserialize MCP configuration '{configurationPath}'.");

        logger.LogInformation("Loaded mcp config count:{config}", configuration.McpServers.Count);

        var clients = new List<McpClient>();
        var tools = new List<AIFunction>();

        try
        {
            foreach (var (name, serverConfiguration)in configuration.McpServers)
            {
                ct.ThrowIfCancellationRequested();

                logger.LogInformation("Reading mcp config for : {mcpServerName}", name);

                var transport = transportFactory.Create(name, serverConfiguration);

                var client = await McpClient.CreateAsync(transport, cancellationToken: ct);

                clients.Add(client);

                var serverTools = await client.ListToolsAsync(cancellationToken: ct);

                tools.AddRange(serverTools);
            }

            _clients = clients;

            logger.LogInformation("Found {count} mcp clients", _clients.Count);
            logger.LogInformation("Found {count} mcp tools", tools.Count);

            return tools;
        }
        catch
        {
            foreach (var client in clients) await client.DisposeAsync();

            throw;
        }
    }
}