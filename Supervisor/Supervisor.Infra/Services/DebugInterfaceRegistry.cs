using System.Text.Json;
using Microsoft.Extensions.Logging;
using Supervisor.Application.Common.Contracts;
using Supervisor.Application.Common.Settings;
using Supervisor.Domain.Entities;
using Supervisor.Infra.Helpers;

namespace Supervisor.Infra.Services;

public class DebugInterfaceRegistry : IInterfaceRegistry
{
    private readonly List<string> _interfaceBuildDirectories = [];
    private readonly string _interfacesBuildDirectoryPath;
    private readonly InterfacesSettings _interfacesSettings;
    private readonly ILogger<DebugInterfaceRegistry> _logger;
    private readonly SupervisorSettings _settings;

    public DebugInterfaceRegistry(
        SupervisorSettings settings,
        ILogger<DebugInterfaceRegistry> logger,
        InterfacesSettings interfacesSettings)
    {
        _settings = settings;
        _logger = logger;
        _interfacesSettings = interfacesSettings;

        _interfacesBuildDirectoryPath = Path.Combine(
            _settings.SupervisorProjectPath,
            "..",
            "..",
            "Interfaces");

        _interfaceBuildDirectories.AddRange(
        [
            GetBuildDirectory("Interfaces.TelegramBot")
        ]);
    }

    public async Task<List<InterfaceManifest>> GetInterfaces(Version runtimeVersion, CancellationToken ct = default)
    {
        var interfaces = new List<InterfaceManifest>();

        foreach (var interfaceManifestPath in _interfaceBuildDirectories
                     .Select(buildDirectoryPath =>
                         Path.Combine(buildDirectoryPath, _interfacesSettings.ManifestFileName))
                     .TakeWhile(File.Exists))
        {
            var manifestString = await File.ReadAllTextAsync(interfaceManifestPath, ct);
            var manifest = JsonSerializer.Deserialize<InterfaceManifest>(manifestString);

            interfaces.Add(manifest!);
        }

        return interfaces;
    }

    public async Task<InterfaceManifest?> GetAsync(string uniqueName, Version interfaceVersion, Version runtimeVersion,
        CancellationToken ct = default)
    {
        var interfaces = await GetInterfaces(runtimeVersion, ct);

        return interfaces.SingleOrDefault(i =>
            i.Version == interfaceVersion &&
            i.RequiredRuntimeVersion <= runtimeVersion &&
            i.UniqueName.Equals(uniqueName, StringComparison.CurrentCultureIgnoreCase));
    }

    public async Task FetchAsync(string uniqueName, Version version, IProgress<ProgressUpdate>? progress = null)
    {
        var buildDirectory = GetInterfaceBuildDirectory(uniqueName, version);

        await FileHelpers.CopyDirectoryAsync(
            buildDirectory,
            _interfacesSettings.GenerateInstallationDirectory(uniqueName),
            progress);
    }

    private string GetInterfaceBuildDirectory(
        string uniqueName,
        Version version)
    {
        _logger.LogDebug("getting interface manifests");

        _logger.LogDebug("finding manifest files");

        var manifests = _interfaceBuildDirectories.Select(buildPath => new
        {
            BuildPath = buildPath,
            InterfaceManifest = JsonSerializer
                .Deserialize<InterfaceManifest>(File
                    .ReadAllText(Path.Combine(buildPath, _interfacesSettings.ManifestFileName)))
        }).ToList();

        _logger.LogDebug($"{manifests.Count} manifests found");

        _logger.LogDebug("filtering manifests");

        var filteredManifest = manifests
            .Where(a =>
                a.InterfaceManifest!.UniqueName == uniqueName &&
                a.InterfaceManifest.Version == version)
            .Select(a => a.BuildPath)
            .Single();

        return filteredManifest;
    }

    private string GetBuildDirectory(string projectDirectoryName)
    {
        return Path.Combine(_interfacesBuildDirectoryPath,
            projectDirectoryName,
            "bin",
            "Debug",
            "net10.0");
    }
}