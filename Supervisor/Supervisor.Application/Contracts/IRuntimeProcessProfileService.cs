using Supervisor.Application.Options;

namespace Supervisor.Application.Contracts;

public interface IRuntimeProcessProfileService
{
    bool ProfileExists();
    Task UpdateProfileAsync(RuntimeProcessProfile profile, CancellationToken ct);
    Task<RuntimeProcessProfile> GetProfileAsync(CancellationToken ct);
}