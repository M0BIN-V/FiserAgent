using System.Text.Json;
using Supervisor.Application.Common.Contracts;
using Supervisor.Application.Common.Options;
using Supervisor.Application.Common.Settings;
using Supervisor.Infra.Helpers;

namespace Supervisor.Infra.Services;

public class DebugRuntimeRegistry : IRuntimeRegistry
{
    private readonly string _runtimeBuildFolder;
    private readonly RuntimeSettings _runtimeSettings;

    public DebugRuntimeRegistry(
        RuntimeSettings runtimeSettings,
        SupervisorSettings supervisorSettings)
    {
        _runtimeSettings = runtimeSettings;


        _runtimeBuildFolder = Path.Combine(
            supervisorSettings.SupervisorProjectPath,
            "..",
            "..",
            "Runtime",
            "Runtime.WebApi",
            "bin",
            "Debug",
            "net10.0");
    }

    public async Task<Version> GetLatestRuntimeVersionAsync()
    {
        var runtimeBuildManifest = Path.Combine(_runtimeBuildFolder, _runtimeSettings.ManifestFileName);

        var manifestString = await File.ReadAllTextAsync(runtimeBuildManifest);
        var manifest = JsonSerializer.Deserialize<RuntimeManifest>(manifestString);

        return new Version(manifest!.Version);
    }

    public async Task FetchRuntimeAsync(Version version, IProgress<ProgressUpdate>? progress = null)
    {
        var runtimeFolder = _runtimeSettings.DirectoryPath;
        await FileHelpers.CopyDirectoryAsync(_runtimeBuildFolder, runtimeFolder, progress);
    }
}