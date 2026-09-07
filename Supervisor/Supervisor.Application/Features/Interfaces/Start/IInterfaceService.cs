namespace Supervisor.Application.Features.Interfaces.Start;

public interface IInterfaceService
{
    public List<InterfaceManifest> GetInstalledInterfaces();
    public bool IsInstalled(string interfaceUniqueName);
    public Task<InterfaceManifest?> GetByUniqueNameAsync(string interfaceUniqueName);
}