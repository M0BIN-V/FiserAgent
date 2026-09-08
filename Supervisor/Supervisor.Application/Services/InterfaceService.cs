using System.Text.Json;
using Supervisor.Application.Common.Settings;
using Supervisor.Application.Features.Interfaces.Start;

namespace Supervisor.Application.Services;

public sealed class InterfaceService(InterfacesSettings settings) : IInterfaceService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<InterfaceManifest?> GetByUniqueNameAsync(string interfaceUniqueName, CancellationToken ct)
    {
        foreach (var directory in GetInterfaceDirectories())
        {
            ct.ThrowIfCancellationRequested();

            var manifest = await ReadManifestAsync(directory, ct);

            if (manifest.UniqueName.Equals(interfaceUniqueName, StringComparison.OrdinalIgnoreCase))
                return manifest;
        }

        return null;
    }

    public async Task<List<InterfaceManifest>> GetInstalledInterfacesAsync(CancellationToken ct)
    {
        var result = new List<InterfaceManifest>();

        foreach (var directory in GetInterfaceDirectories())
        {
            ct.ThrowIfCancellationRequested();

            var manifest = await ReadManifestAsync(directory, ct);

            result.Add(manifest);
        }

        return result;
    }

    public async Task<string?> GetInterfaceDirectoryPathAsync(string interfaceUniqueName, CancellationToken ct)
    {
        foreach (var directory in GetInterfaceDirectories())
        {
            ct.ThrowIfCancellationRequested();

            var manifest = await ReadManifestAsync(directory, ct);

            if (manifest.UniqueName.Equals(interfaceUniqueName, StringComparison.OrdinalIgnoreCase))
                return directory;
        }

        return null;
    }

    private IEnumerable<string> GetInterfaceDirectories()
    {
        return !Directory.Exists(settings.InstallationDirectory)
            ? throw new DirectoryNotFoundException(settings.InstallationDirectory)
            : Directory.EnumerateDirectories(settings.InstallationDirectory);
    }

    private async Task<InterfaceManifest> ReadManifestAsync(string interfaceDirectory, CancellationToken ct)
    {
        var manifestPath = Path.Combine(interfaceDirectory, settings.ManifestFileName);

        if (!File.Exists(manifestPath)) throw new InterfaceManifestNotFoundException(manifestPath);

        try
        {
            await using var stream = File.OpenRead(manifestPath);

            var manifest = await JsonSerializer.DeserializeAsync<InterfaceManifest>(
                stream,
                JsonOptions,
                ct);

            return manifest ?? throw new InvalidInterfaceManifestException(manifestPath);
        }
        catch (JsonException ex)
        {
            throw new InvalidInterfaceManifestException(manifestPath, ex);
        }
    }
}

internal class InvalidInterfaceManifestException(string manifestPath, Exception? inner = null)
    : Exception($"cant read interface from {manifestPath}", inner);

internal class InterfaceManifestNotFoundException(string manifestPath, Exception? inner = null) :
    Exception($"cant find manifest in {manifestPath}", inner);