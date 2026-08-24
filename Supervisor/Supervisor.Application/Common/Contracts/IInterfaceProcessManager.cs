namespace Supervisor.Application.Common.Contracts;

public interface IInterfaceProcessManager
{
    Task<bool> InterfaceIsRunningAsync(CancellationToken ct);
    Task ShutdownInterfaceAsync(CancellationToken ct);
}