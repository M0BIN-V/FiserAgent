using System.Text.Json;
using Fiser.Supervisor.Options;
using Microsoft.Extensions.Options;

namespace Fiser.Supervisor.Services;

public class RuntimeProcessProfileService(IOptions<RuntimeOptions> options) :
    IRuntimeProcessProfileService
{
    public bool ProfileExists()
    {
        return File.Exists(options.Value.ProcessProfile);
    }

    public async Task<RuntimeProcessProfile> GetProfileAsync(CancellationToken ct)
    {
        var profileStr = await File.ReadAllTextAsync(options.Value.ProcessProfile, ct);
        var profile = JsonSerializer.Deserialize<RuntimeProcessProfile>(profileStr);

        return profile ?? throw new NullReferenceException("No profile found");
    }

    public async Task UpdateProfileAsync(RuntimeProcessProfile profile, CancellationToken ct)
    {
        var profileStr = JsonSerializer.Serialize(profile);
        await File.WriteAllTextAsync(options.Value.ProcessProfile, profileStr, ct);
    }
}