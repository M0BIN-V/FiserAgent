namespace Fiser.Supervisor.Services;

public interface IRuntimeService
{
    public Task<Version?> GetRuntimeVersionAsync();
    public bool RunIsTimeInstalled();
}