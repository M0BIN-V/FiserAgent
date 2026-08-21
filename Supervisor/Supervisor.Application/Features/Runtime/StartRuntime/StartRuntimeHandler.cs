using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Supervisor.Application.Common.Options;
using Supervisor.Application.Services;

namespace Supervisor.Application.Features.Runtime.StartRuntime;

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

        if (!await runtimeProcessManager.RespondsHealthyAsync(CancellationToken.None))
            throw new Exception("Runtime did not respond healthy after starting.");

        return new StartRuntimeResponse(new Uri(profile.Url));
    }
}