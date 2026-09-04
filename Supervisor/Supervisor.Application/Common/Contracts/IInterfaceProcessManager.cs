namespace Supervisor.Application.Common.Contracts;

public interface IInterfaceProcessManager
{
    Task<bool> InterfaceIsRunningAsync(string interfaceUniqueName,CancellationToken ct);
    Task ShutdownInterfaceAsync(CancellationToken ct);
}