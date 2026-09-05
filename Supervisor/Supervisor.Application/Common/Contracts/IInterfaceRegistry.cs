using Supervisor.Domain.Entities;

namespace Supervisor.Application.Common.Contracts;

public interface IInterfaceRegistry
{
    public Task<List<Interface>> GetInterfaces(Version runtimeVersion, CancellationToken ct = default);

    Task<Interface?> GetAsync(
        string uniqueName,
        Version interfaceVersion,
        Version runtimeVersion,
        CancellationToken ct = default);

    Task FetchAsync(string uniqueName, Version version, IProgress<ProgressUpdate>? progress = null);
}