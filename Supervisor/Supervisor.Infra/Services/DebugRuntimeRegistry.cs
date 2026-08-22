using System.Text.Json;
using Microsoft.Extensions.Options;
using Supervisor.Application.Common.Contracts;
using Supervisor.Application.Common.Options;
using Supervisor.Infra.Helpers;

namespace Supervisor.Infra.Services;

public class DebugRuntimeRegistry : IRuntimeRegistry
{
    private readonly string _runtimeBuildFolder;
    private readonly RuntimeOptions _runtimeOptions;

    public DebugRuntimeRegistry(
        IOptions<RuntimeOptions> runtimeOptions,
        IOptions<SupervisorOptions> supervisorOptions)
    {
        _runtimeOptions = runtimeOptions.Value;


        _runtimeBuildFolder = Path.Combine(
            supervisorOptions.Value.SupervisorProjectPath,
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
        var runtimeBuildManifest = Path.Combine(_runtimeBuildFolder, _runtimeOptions.ManifestFileName);

        var manifestString = await File.ReadAllTextAsync(runtimeBuildManifest);
        var manifest = JsonSerializer.Deserialize<RuntimeManifest>(manifestString);

        return new Version(manifest!.Version);
    }

    public async Task FetchRuntimeAsync(Version version, IProgress<double>? progress = null)
    {
        var runtimeFolder = _runtimeOptions.FolderPath;
        await FileHelpers.CopyDirectoryAsync(_runtimeBuildFolder, runtimeFolder, progress);
    }
}