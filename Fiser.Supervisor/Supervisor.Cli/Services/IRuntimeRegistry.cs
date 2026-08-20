namespace Fiser.Supervisor.Cli.Services;

public interface IRuntimeRegistry
{
    public Task<Version> GetLatestRuntimeVersionAsync();
    public Task FetchRuntimeAsync(IProgress<double> progress);
}