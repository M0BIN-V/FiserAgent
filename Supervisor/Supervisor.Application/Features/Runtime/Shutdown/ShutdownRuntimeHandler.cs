using Microsoft.Extensions.Logging;
using Supervisor.Application.Features.Shutdown;
using Supervisor.Application.Services;

namespace Supervisor.Application.Features.Runtime.Shutdown;

public record ShutdownRuntimeResponse(bool runtimeWasNotRunning);

public class ShutdownRuntimeHandler(
    ILogger<ShutdownRuntimeHandler> logger,
    RuntimeProcessManager runtimeProcessManager) : Handler<ShutdownRuntimeRequest, ShutdownRuntimeResponse>
{
    public override async Task<ShutdownRuntimeResponse> HandleAsync(ShutdownRuntimeRequest runtimeRequest, CancellationToken ct = default)
    {
        logger.LogInformation("Finding runtime process...");

        if (!await runtimeProcessManager.IsRunningHealthyAsync(CancellationToken.None))
        {
            logger.LogWarning("Runtime is not running.");
            return new ShutdownRuntimeResponse(true);
        }

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        await runtimeProcessManager.ShutdownAsync(timeout.Token);

        return new ShutdownRuntimeResponse(false);
    }
}