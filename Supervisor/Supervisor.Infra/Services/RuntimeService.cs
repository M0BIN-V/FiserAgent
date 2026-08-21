using System.Text.Json;
using Microsoft.Extensions.Options;
using Supervisor.Application.Common.Contracts;
using Supervisor.Application.Common.Options;

namespace Supervisor.Infra.Services;

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