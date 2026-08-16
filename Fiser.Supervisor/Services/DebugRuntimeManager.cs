namespace Fiser.Supervisor.Services;

public class DebugRuntimeManager : IRuntimeManager
{
    public Version? GetCurrentRuntimeVersion()
    {
        return new Version("1.1.1");
    }

    public Version GetLastRuntimeVersion()
    {
        return new Version("1.2.1");
    }
}
