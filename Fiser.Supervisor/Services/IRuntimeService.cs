namespace Fiser.Supervisor.Services;

public interface IRuntimeService
{
    public Task<Version?> GetRuntimeVersionAsync();

    public Task<Version> GetLatestRuntimeVersionAsync();

    public bool RunIsTimeInstalled();

    public Task FetchRuntimeAsync(IProgress<double> progress);
}