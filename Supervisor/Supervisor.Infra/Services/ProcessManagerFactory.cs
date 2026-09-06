using Microsoft.Extensions.Logging;
using Supervisor.Application.Common.Contracts;
using Supervisor.Application.Common.Contracts.Process;
using Supervisor.Application.Services.Process;

namespace Supervisor.Infra.Services;

public class ProcessManagerFactory(
    ILogger<ProcessManager> processManagerLogger,
    IPipeClient pipeClient) : IProcessManagerFactory
{
    public IProcessManager Create(ProcessProfile profile)
    {
        return new ProcessManager(profile, processManagerLogger, pipeClient);
    }
}