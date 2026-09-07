namespace Supervisor.Application.Common.Contracts;

public interface IInterfaceRegistry
{
    public Task<List<InterfaceManifest>> GetInterfaces(Version runtimeVersion, CancellationToken ct = default);

    Task<InterfaceManifest?> GetAsync(
        string uniqueName,
        Version interfaceVersion,
        Version runtimeVersion,
        CancellationToken ct = default);

    Task FetchAsync(string uniqueName, Version version, IProgress<ProgressUpdate>? progress = null);
}