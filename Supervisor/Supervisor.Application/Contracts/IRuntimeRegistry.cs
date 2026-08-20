namespace Supervisor.Application.Contracts;

public interface IRuntimeRegistry
{
    public Task<Version> GetLatestRuntimeVersionAsync();
    public Task FetchRuntimeAsync(IProgress<double> progress);
}