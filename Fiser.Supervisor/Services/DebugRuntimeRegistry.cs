using System.Text.Json;
using Fiser.Supervisor.Helpers;
using Fiser.Supervisor.Options;
using Microsoft.Extensions.Options;

namespace Fiser.Supervisor.Services;

public class DebugRuntimeRegistry : IRuntimeRegistry
{
    private readonly string _runtimeBuildFolder;
    private readonly RuntimeOptions _runtimeOptions;

    public DebugRuntimeRegistry(
        IOptions<RuntimeOptions> runtimeOptions,
        IOptions<SupervisorOptions> supervisorOptions)
    {
        
        _runtimeOptions = runtimeOptions.Value;
        var supervisorOptions1 = supervisorOptions.Value;

        var supervisorProjectPath = Path.Combine(supervisorOptions1.Directory, "..", "..", "..");

        _runtimeBuildFolder = Path.Combine(
            supervisorProjectPath,
            "..",
            "Fiser.Runtime",
            "Fiser.Runtime.WebApi",
            "bin",
            "Debug",
            "net10.0");
    }

    public async Task<Version> GetLatestRuntimeVersionAsync()
    {
        var manifestString = await File.ReadAllTextAsync(_runtimeOptions.ManifestPath);
        var manifest = JsonSerializer.Deserialize<RuntimeManifest>(manifestString);

        return new Version(manifest!.Version);
    }

    public async Task FetchRuntimeAsync(IProgress<double> progress)
    {
        var runtimeFolder = _runtimeOptions.FolderPath;
        await FileHelpers.CopyDirectoryAsync(_runtimeBuildFolder, runtimeFolder, progress);
    }
}