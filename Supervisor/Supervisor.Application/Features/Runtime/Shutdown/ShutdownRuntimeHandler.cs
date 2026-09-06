using Supervisor.Application.Common.Contracts.Process;
using Supervisor.Application.Features.Shutdown;
using Supervisor.Application.Services.Process;
using Supervisor.Application.Services.Process.Runtime;

namespace Supervisor.Application.Features.Runtime.Shutdown;

public record ShutdownRuntimeResponse(bool runtimeWasNotRunning);

public class ShutdownRuntimeHandler(
    ILogger<ShutdownRuntimeHandler> logger,
    IProcessManagerFactory managerFactory,
    RuntimeProfileService profileService,
    RuntimeClient runtimeClient) : Handler<ShutdownRuntimeRequest, ShutdownRuntimeResponse>
{
    public override async Task<ShutdownRuntimeResponse> HandleAsync(ShutdownRuntimeRequest runtimeRequest,
        CancellationToken ct = default)
    {
        logger.LogInformation("Finding runtime process...");

        if (!await runtimeClient.RespondsHealthyAsync(CancellationToken.None))
        {
            logger.LogWarning("Runtime is not running.");
            return new ShutdownRuntimeResponse(true);
        }

        var profile = await profileService.GetProfileAsync(ct);
        var manager = managerFactory.Create(profile);

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        await manager.ShutdownAsync(timeout.Token);

        return new ShutdownRuntimeResponse(false);
    }
}