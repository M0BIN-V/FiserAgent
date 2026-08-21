using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Supervisor.Application.Common.Abstractions;
using Supervisor.Application.Common.Contracts;
using Supervisor.Application.Common.Options;
using Supervisor.Application.Services;

namespace Supervisor.Application.Features.StartRuntime;

public record StartRuntimeResponse(Uri Endpoint);

public class StartRuntimeHandler(
    IRuntimeProcessProfileService profileService,
    RuntimeProcessManager runtimeProcessManager,
    IOptions<RuntimeOptions> runtimeOptions,
    ILogger<StartRuntimeHandler> logger) : Handler<StartRuntimeRequest, StartRuntimeResponse>
{
    public override async Task<StartRuntimeResponse> HandleAsync(StartRuntimeRequest request,
        CancellationToken ct = default)
    {
        await runtimeProcessManager.StartAsync(runtimeOptions.Value.FilePath, ct);

        while (true)
        {
            var canRead = runtimeProcessManager.Output.Reader.TryRead(out var line);
            if (!canRead) break;

            logger.LogDebug(line ?? " ");
        }

        var profile = await profileService.GetProfileAsync(ct);
        return new StartRuntimeResponse(new Uri(profile.Url));
    }
}