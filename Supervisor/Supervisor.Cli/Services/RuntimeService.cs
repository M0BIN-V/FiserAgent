using System.Text.Json;
using Fiser.Supervisor.Cli.Services;
using Microsoft.Extensions.Options;
using Supervisor.Cli.Options;

namespace Supervisor.Cli.Services;

public class RuntimeService(IOptions<RuntimeOptions> runtimeOptions) : IRuntimeService
{
    private readonly RuntimeOptions _runtimeOptions = runtimeOptions.Value;

    public async Task<Version?> GetRuntimeVersionAsync()
    {
        var manifestString = await File.ReadAllTextAsync(_runtimeOptions.ManifestPath);

        if (string.IsNullOrWhiteSpace(manifestString))
            return null;

        var manifest = JsonSerializer.Deserialize<RuntimeManifest>(manifestString);

        if (manifest is null) return null;

        return new Version(manifest.Version);
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
}