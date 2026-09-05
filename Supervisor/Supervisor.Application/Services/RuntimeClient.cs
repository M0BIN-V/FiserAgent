using System.Net;
using Supervisor.Application.Services.ProcessProfile;

namespace Supervisor.Application.Services;

public sealed class RuntimeClient(RuntimeProfileService profileService, HttpClient client)
{
    public async Task<bool> RespondsHealthyAsync(CancellationToken ct)
    {
        var profile = await profileService.GetProfileAsync(ct);

        var baseUrl = new Uri(profile.Url ?? throw new InvalidOperationException());

        var aliveUrl = new Uri(baseUrl, "alive");

        var response = await client.GetAsync(aliveUrl, ct);

        return response.StatusCode is HttpStatusCode.OK;
    }
}