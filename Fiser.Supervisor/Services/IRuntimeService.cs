namespace Fiser.Supervisor.Services;

public interface IRuntimeService
{
    public Task<Version?> GetRuntimeVersionAsync();
    public Version GetLatestRuntimeVersion();

    public bool RunIsTimeInstalled();
    public Task FetchRuntimeAsync(IProgress<double> progress);
}