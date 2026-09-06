using System.Diagnostics;

namespace Supervisor.Application.Common.Contracts.Process;

public interface IProcessManager
{
    public Task<bool> IsRunningHealthyAsync(CancellationToken ct);

    public Task<System.Diagnostics.Process> StartProcess(
        string filePath,
        Dictionary<string, string> environmentVariables,
        DataReceivedEventHandler? onOutput = null,
        DataReceivedEventHandler? onError = null,
        CancellationToken ct = default);

    public Task ShutdownAsync(CancellationToken ct);
}