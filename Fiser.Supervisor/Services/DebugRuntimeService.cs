using System.Text.Json;
using Fiser.Supervisor.Helpers;
using Fiser.Supervisor.Options;
using Microsoft.Extensions.Options;

namespace Fiser.Supervisor.Services;

public class DebugRuntimeService : IRuntimeService
{
    private readonly string _runtimeBuildFolder;
    private readonly RuntimeOptions _runtimeOptions;
    private readonly SupervisorOptions _supervisorOptions;
    private readonly string _supervisorProjectPath;

    public DebugRuntimeService(
        IOptions<RuntimeOptions> runtimeOptions,
        IOptions<SupervisorOptions> supervisorOptions)
    {
        _runtimeOptions = runtimeOptions.Value;
        _supervisorOptions = supervisorOptions.Value;

        _supervisorProjectPath = Path.Combine(_supervisorOptions.Directory, "..", "..", "..");

        _runtimeBuildFolder = Path.Combine(
            _supervisorProjectPath,
            "..",
            "Fiser.Runtime",
            "Fiser.Runtime.WebApi",
            "bin",
            "Debug",
            "net10.0");
    }

    public async Task<Version?> GetRuntimeVersionAsync()
    {
        var manifestString = await File.ReadAllTextAsync(_runtimeOptions.ManifestPath);

        if (string.IsNullOrWhiteSpace(manifestString))
            return null;

        var manifest = JsonSerializer.Deserialize<RuntimeManifest>(manifestString);

        if (manifest is null) return null;

        return new Version(manifest.Version);
    }

    public async Task<Version> GetLatestRuntimeVersionAsync()
    {
        var manifestString = await File.ReadAllTextAsync(Path.Combine(_runtimeBuildFolder, "runtime.json"));
        var manifest = JsonSerializer.Deserialize<RuntimeManifest>(manifestString);

        return new Version(manifest!.Version);
    }

    public bool RunIsTimeInstalled()
    {
        var runtimeFolder = _runtimeOptions.FolderPath;

        if (!Directory.Exists(runtimeFolder))
            return false;

        var runtimeFile = _runtimeOptions.FilePath;

        if (!File.Exists(runtimeFile))
            return false;

        return true;
    }

    public async Task FetchRuntimeAsync(IProgress<double> progress)
    {
        //Copy runtime exe file from project 
        var runtimeFolder = _runtimeOptions.FolderPath;

        await FileHelpers.CopyDirectoryAsync(_runtimeBuildFolder, runtimeFolder, progress);
    }
}