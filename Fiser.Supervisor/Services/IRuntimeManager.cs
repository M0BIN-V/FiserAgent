namespace Fiser.Supervisor.Services;

public interface IRuntimeManager
{
    public Version? GetCurrentRuntimeVersion();
    public Version GetLastRuntimeVersion();
}