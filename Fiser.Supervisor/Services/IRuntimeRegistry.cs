namespace Fiser.Supervisor.Services;

public interface IRuntimeRegistry
{
    public Task<Version> GetLatestRuntimeVersionAsync();
    public Task FetchRuntimeAsync(IProgress<double> progress);
}