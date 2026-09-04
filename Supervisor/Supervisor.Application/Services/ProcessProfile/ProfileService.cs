using System.Text.Json;
using Microsoft.Extensions.Options;
using Supervisor.Application.Common.Options;

namespace Supervisor.Application.Services.ProcessProfile;

public class RuntimeProfileService(IOptions<RuntimeOptions> options) : ProfileService<RuntimeProcessProfile>
{
    protected override string ProfileFilePath { get; } = options.Value.ProcessProfile;
}

public abstract class ProfileService<TProcessProfile>
    where TProcessProfile : ProcessProfile
{
    protected abstract string ProfileFilePath { get; }

    public async Task<TProcessProfile> GetProfileAsync(CancellationToken ct)
    {
        var strProfile = await File.ReadAllTextAsync(ProfileFilePath, ct);

        var profile = JsonSerializer.Deserialize<TProcessProfile>(strProfile);

        return profile ?? throw new InvalidCastException("Failed to deserialize process profile.");
    }

    public async Task UpdateProfileAsync(TProcessProfile profile, CancellationToken ct)
    {
        var strProfile = JsonSerializer.Serialize(profile);

        await File.WriteAllTextAsync(ProfileFilePath, strProfile, ct);
    }

    public bool ProfileExists()
    {
        return File.Exists(ProfileFilePath);
    }
}