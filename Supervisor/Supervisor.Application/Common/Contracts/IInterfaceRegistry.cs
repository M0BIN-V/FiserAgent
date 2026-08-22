using Supervisor.Domain.Entities;

namespace Supervisor.Application.Common.Contracts;

public interface IInterfaceRegistry
{
    public Task<List<Interface>> GetInterfaces(Version runtimeVersion ,CancellationToken ct = default);
}