namespace Fiser.Supervisor.Cli.Services;

public interface IRuntimeService
{
    public Task<Version?> GetRuntimeVersionAsync();
    public bool RunIsTimeInstalled();
}