namespace Supervisor.Application.Common.Contracts;

public interface IRuntimeRegistry
{
    public Task<Version> GetLatestRuntimeVersionAsync();
    public Task FetchRuntimeAsync(IProgress<double> progress);
}