using System.Text.Json;
using Microsoft.Extensions.Options;
using Supervisor.Application.Common.Contracts;
using Supervisor.Application.Common.Options;
using Supervisor.Domain.Entities;
using Supervisor.Infra.Helpers;

namespace Supervisor.Infra.Services;

public class DebugInterfaceRegistry : IInterfaceRegistry
{
    private readonly SupervisorOptions _supervisorOptions;
    private readonly string _telegramBuildPath;

    public DebugInterfaceRegistry(IOptions<SupervisorOptions> supervisorOptions)
    {
        _supervisorOptions = supervisorOptions.Value;

        _telegramBuildPath = Path.Combine(
            _supervisorOptions.SupervisorProjectPath,
            "..",
            "..",
            "Interfaces",
            "Interfaces.TelegramBot",
            "bin",
            "Debug",
            "net10.0");
    }

    public Task<List<Interface>> GetInterfaces(Version runtimeVersion, CancellationToken ct = default)
    {
        var interfaces = new List<Interface>();

        var interfaceManifestPath = Path.Combine(_telegramBuildPath, "interface.json");

        if (!File.Exists(interfaceManifestPath)) return Task.FromResult(interfaces);

        var manifestString = File.ReadAllText(interfaceManifestPath);
        var manifest = JsonSerializer.Deserialize<Interface>(manifestString);

        interfaces.Add(manifest!);

        return Task.FromResult(interfaces);
    }
}