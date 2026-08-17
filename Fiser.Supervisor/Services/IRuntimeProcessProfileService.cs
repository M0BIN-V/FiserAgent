using Fiser.Supervisor.Options;

namespace Fiser.Supervisor.Services;

public interface IRuntimeProcessProfileService
{
    bool ProfileExistsAsync();
    Task UpdateProfileAsync(RuntimeProcessProfile profile, CancellationToken ct);
    Task<RuntimeProcessProfile> GetProfileAsync();
}