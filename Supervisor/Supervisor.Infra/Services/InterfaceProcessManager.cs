using Supervisor.Application.Common.Contracts;

namespace Supervisor.Infra.Services;

public class InterfaceProcessManager : IInterfaceProcessManager
{
    public Task<bool> InterfaceIsRunningAsync(CancellationToken ct)
    {
        return
            Task.FromResult(false);
    }

    public Task ShutdownInterfaceAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}