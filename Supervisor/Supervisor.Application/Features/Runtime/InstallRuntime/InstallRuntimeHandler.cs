using Supervisor.Application.Services;

namespace Supervisor.Application.Features.Runtime.InstallRuntime;

public record InstallRuntimeRequest(Version Version, IProgress<double>? progress = null);

public record InstallRuntimeResponse;

public class InstallRuntimeHandler(
    IRuntimeRegistry registry,
    RuntimeProcessManager processManager) : Handler<InstallRuntimeRequest, InstallRuntimeResponse>
{
    public override async Task<InstallRuntimeResponse> HandleAsync(InstallRuntimeRequest request,
        CancellationToken ct = default)
    {
        if (await processManager.IsRunningAsync(ct)) await processManager.ShutdownAsync(ct);

        await registry.FetchRuntimeAsync(request.Version, request.progress);

        return new InstallRuntimeResponse();
    }
}