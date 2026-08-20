using Supervisor.Cli.Options;

namespace Supervisor.Cli.Services;

public interface IRuntimeProcessProfileService
{
    bool ProfileExists();
    Task UpdateProfileAsync(RuntimeProcessProfile profile, CancellationToken ct);
    Task<RuntimeProcessProfile> GetProfileAsync(CancellationToken ct);
}