using System.Text.Json;
using Microsoft.Extensions.Options;
using Supervisor.Application.Common.Contracts;
using Supervisor.Application.Common.Options;
using Supervisor.Domain.Entities;
using Supervisor.Infra.Helpers;

namespace Supervisor.Infra.Services;

public class DebugInterfaceRegistry : IInterfaceRegistry
{
    private readonly List<string> _interfaceBuildDirectories = [];
    private readonly string _interfacesDirectoryPath;
    private readonly SupervisorOptions _supervisorOptions;

    public DebugInterfaceRegistry(IOptions<SupervisorOptions> supervisorOptions)
    {
        _supervisorOptions = supervisorOptions.Value;

        _interfacesDirectoryPath = Path.Combine(
            _supervisorOptions.SupervisorProjectPath,
            "..",
            "..",
            "Interfaces");

        _interfaceBuildDirectories.AddRange(
        [
            GetBuildDirectory("Interfaces.TelegramBot")
        ]);
    }

    public async Task<List<Interface>> GetInterfaces(Version runtimeVersion, CancellationToken ct = default)
    {
        var interfaces = new List<Interface>();

        foreach (var interfaceManifestPath in _interfaceBuildDirectories
                     .Select(buildDirectoryPath => Path.Combine(buildDirectoryPath, "interface.json"))
                     .TakeWhile(File.Exists))
        {
            var manifestString = await File.ReadAllTextAsync(interfaceManifestPath, ct);
            var manifest = JsonSerializer.Deserialize<Interface>(manifestString);

            interfaces.Add(manifest!);
        }

        return interfaces;
    }

    public async Task<Interface?> GetAsync(string uniqueName, Version interfaceVersion, Version runtimeVersion,
        CancellationToken ct = default)
    {
        var interfaces = await GetInterfaces(runtimeVersion, ct);

        return interfaces.SingleOrDefault(i =>
            i.Version == interfaceVersion &&
            i.RequiredRuntimeVersion == runtimeVersion &&
            i.UniqueName.Equals(uniqueName, StringComparison.CurrentCultureIgnoreCase));
    }

    public async Task FetchAsync(string uniqueName, Version version, IProgress<double>? progress = null)
    {
        var buildDirectory = GetInterfaceBuildDirectory(uniqueName, version);

        await FileHelpers.CopyDirectoryAsync(
            buildDirectory,
            _supervisorOptions.InterfaceInstallationPath,
            progress);
    }

    private string GetInterfaceBuildDirectory(
        string uniqueName,
        Version version)
    {
        return _interfaceBuildDirectories
            .Select(buildPath => new
            {
                BuildPath = buildPath,
                InterfaceManifest = JsonSerializer
                    .Deserialize<Interface>(File
                        .ReadAllText(Path.Combine(buildPath, "interface.json")))
            })
            .Where(a =>
                a.InterfaceManifest!.UniqueName == uniqueName &&
                a.InterfaceManifest.Version == version)
            .Select(a => a.BuildPath)
            .Single();
    }

    private string GetBuildDirectory(string projectDirectoryName)
    {
        return Path.Combine(_interfacesDirectoryPath,
            projectDirectoryName,
            "bin",
            "Debug",
            "net10.0");
    }
}