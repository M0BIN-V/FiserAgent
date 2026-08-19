using Fiser.Supervisor.Options;

namespace Fiser.Supervisor.Services;

public interface IRuntimeProcessProfileService
{
    bool ProfileExists();
    Task UpdateProfileAsync(RuntimeProcessProfile profile, CancellationToken ct);
    Task<RuntimeProcessProfile> GetProfileAsync(CancellationToken ct);
}