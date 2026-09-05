using Supervisor.Application.Services;
using Supervisor.Application.Services.Process;
using Supervisor.Application.Services.Process.Runtime;

namespace Supervisor.Application.Features.Runtime.InstallRuntime;

public record InstallRuntimeRequest(Version? Version = null, IProgress<ProgressUpdate>? progress = null);

public record InstallRuntimeResponse(Version installedVersion);

public class InstallRuntimeHandler(
    ProcessManagerFactory managerFactory,
    IRuntimeRegistry registry,
    RuntimeProfileService profileService) : Handler<InstallRuntimeRequest, InstallRuntimeResponse>
{
    public override async Task<InstallRuntimeResponse> HandleAsync(InstallRuntimeRequest request,
        CancellationToken ct = default)
    {
        var profile = await profileService.GetProfileAsync(ct);
        var manager = managerFactory.Create(profile);

        if (await manager.IsRunningHealthyAsync(ct)) await manager.ShutdownAsync(ct);

        var version = request.Version ?? await registry.GetLatestRuntimeVersionAsync();

        await registry.FetchRuntimeAsync(version, request.progress);

        return new InstallRuntimeResponse(version);
    }
}