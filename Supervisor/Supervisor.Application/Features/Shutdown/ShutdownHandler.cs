using Microsoft.Extensions.Logging;
using Supervisor.Application.Services;

namespace Supervisor.Application.Features.Shutdown;

public record ShutdownResponse(bool runtimeWasNotRunning);

public class ShutdownHandler(
    ILogger<ShutdownHandler> logger,
    RuntimeProcessManager runtimeProcessManager)
{
    public async Task<ShutdownResponse> HandleAsync()
    {
        logger.LogInformation("Finding runtime process...");

        if (!await runtimeProcessManager.IsRunningAsync(CancellationToken.None))
        {
            logger.LogWarning("Runtime is not running.");
            return new ShutdownResponse(true);
        }

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        await runtimeProcessManager.ShutdownAsync(timeout.Token);

        return new ShutdownResponse(false);
    }
}