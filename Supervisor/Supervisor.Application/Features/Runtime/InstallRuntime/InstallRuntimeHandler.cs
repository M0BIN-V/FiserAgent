using Supervisor.Application.Services;

namespace Supervisor.Application.Features.Runtime.InstallRuntime;

public record InstallRuntimeRequest(Version? Version = null, IProgress<ProgressUpdate>? progress = null);

public record InstallRuntimeResponse(Version installedVersion);

public class InstallRuntimeHandler(
    IRuntimeRegistry registry,
    RuntimeProcessManager processManager) : Handler<InstallRuntimeRequest, InstallRuntimeResponse>
{
    public override async Task<InstallRuntimeResponse> HandleAsync(InstallRuntimeRequest request,
        CancellationToken ct = default)
    {
        
        if (await processManager.IsRunningHealthyAsync(ct)) await processManager.ShutdownAsync(ct);
        
        var version = request.Version ?? await registry.GetLatestRuntimeVersionAsync();

        await registry.FetchRuntimeAsync(version, request.progress);

        return new InstallRuntimeResponse(version);
    }
}