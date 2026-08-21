using System.Text.Json;
using Microsoft.Extensions.Options;
using Supervisor.Application.Common.Contracts;
using Supervisor.Application.Common.Options;

namespace Supervisor.Infra.Services;

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