namespace Supervisor.Application.Contracts;

public interface IRuntimeService
{
    public Task<Version?> GetRuntimeVersionAsync();
    public bool RunIsTimeInstalled();
}