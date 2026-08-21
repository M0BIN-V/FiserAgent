namespace Supervisor.Application.Common.Contracts;

public interface IRuntimeService
{
    public Task<Version?> GetRuntimeVersionAsync();
    public bool RunIsTimeInstalled();
}