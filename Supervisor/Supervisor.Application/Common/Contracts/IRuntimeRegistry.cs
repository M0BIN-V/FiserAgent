namespace Supervisor.Application.Common.Contracts;

public interface IRuntimeRegistry
{
    public Task<Version> GetLatestRuntimeVersionAsync();
    public Task FetchRuntimeAsync(Version version, IProgress<double>? progress = null);
}