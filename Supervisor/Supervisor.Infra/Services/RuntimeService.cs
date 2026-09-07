using System.Text.Json;
using Supervisor.Application.Common.Contracts;
using Supervisor.Application.Common.Options;
using Supervisor.Application.Common.Settings;

namespace Supervisor.Infra.Services;

//TODO Manages the installed runtime and its version.

public class RuntimeService(RuntimeSettings runtimeSettings) : IRuntimeService
{
    public async Task<Version?> GetRuntimeVersionAsync()
    {
        var manifestString = await File.ReadAllTextAsync(runtimeSettings.ManifestPath);

        if (string.IsNullOrWhiteSpace(manifestString))
            return null;

        var manifest = JsonSerializer.Deserialize<RuntimeManifest>(manifestString);

        if (manifest is null) return null;

        return new Version(manifest.Version);
    }

    public bool RunIsTimeInstalled()
    {
        if (!Directory.Exists(runtimeSettings.DirectoryPath)) return false;

        if (!File.Exists(runtimeSettings.BinaryPath)) return false;

        return true;
    }
}