namespace Supervisor.Application.Common.Contracts;

public interface IInterfaceService
{
    public Task<List<InterfaceManifest>> GetInstalledInterfacesAsync(CancellationToken ct);
    public Task<InterfaceManifest?> GetByUniqueNameAsync(string interfaceUniqueName,CancellationToken ct);
    public Task<string?> GetInterfaceDirectoryPathAsync(string interfaceUniqueName, CancellationToken ct);
}