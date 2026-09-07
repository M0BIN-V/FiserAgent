using Supervisor.Application.Common.Contracts.Process;
using Supervisor.Application.Services.Process.Runtime;

namespace Supervisor.Application.Features.Runtime.InstallRuntime;

public record InstallRuntimeRequest(Version? Version = null, IProgress<ProgressUpdate>? progress = null);

public record InstallRuntimeResponse(Version installedVersion);

public class InstallRuntimeHandler(
    ILogger<InstallRuntimeHandler> logger,
    IProcessManagerFactory managerFactory,
    IRuntimeRegistry registry,
    RuntimeProfileService profileService) : Handler<InstallRuntimeRequest, InstallRuntimeResponse>
{
    public override async Task<InstallRuntimeResponse> HandleAsync(InstallRuntimeRequest request,
        CancellationToken ct = default)
    {
        logger.LogDebug("Reading runtime process profile");
        if (profileService.ProfileExists())
        {
            var profile = await profileService.GetProfileAsync(ct);
            var manager = managerFactory.Create(profile);

            if (await manager.IsRunningHealthyAsync(ct))
            {
                logger.LogDebug("shutting down runtime");
                await manager.ShutdownAsync(ct);
            }
        }
        else
        {
            logger.LogDebug("Runtime process profile not found");
        }

        var version = request.Version ?? await registry.GetLatestRuntimeVersionAsync();

        await registry.FetchRuntimeAsync(version, request.progress);

        return new InstallRuntimeResponse(version);
    }
}