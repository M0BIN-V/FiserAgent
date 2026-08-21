using Supervisor.Application.Common.Options;

namespace Supervisor.Application.Common.Contracts;

public interface IRuntimeProcessProfileService
{
    bool ProfileExists();
    Task UpdateProfileAsync(RuntimeProcessProfile profile, CancellationToken ct);
    Task<RuntimeProcessProfile> GetProfileAsync(CancellationToken ct);
}