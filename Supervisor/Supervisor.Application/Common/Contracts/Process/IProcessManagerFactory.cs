using Supervisor.Application.Services.Process;

namespace Supervisor.Application.Common.Contracts.Process;

public interface IProcessManagerFactory
{
    public IProcessManager Create(ProcessProfile profile);
}