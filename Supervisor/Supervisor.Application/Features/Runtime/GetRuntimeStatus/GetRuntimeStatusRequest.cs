using Supervisor.Application.Common.Contracts.Process;
using Supervisor.Application.Services.Process;
using Supervisor.Application.Services.Process.Runtime;

namespace Supervisor.Application.Features.Runtime.GetRuntimeStatus;

public record GetRuntimeStatusRequest;

public record GetRuntimeStatusResponse(bool Installed, Version? Version, bool IsRunning, Uri? Endpoint);

public class GetRuntimeStatusHandler(
    RuntimeProfileService profileService,
    IRuntimeService runtimeService,
    IProcessManagerFactory managerFactory,
    RuntimeClient runtimeClient) :
    Handler<GetRuntimeStatusRequest, GetRuntimeStatusResponse>
{
    public override async Task<GetRuntimeStatusResponse> HandleAsync(GetRuntimeStatusRequest request,
        CancellationToken ct = default)
    {
        var isInstalled = runtimeService.RunIsTimeInstalled();

        if (!isInstalled) return new GetRuntimeStatusResponse(isInstalled, null, false, null);

        var profile = await profileService.GetProfileAsync(ct);

        var manager = managerFactory.Create(profile);
        var processIsRunning = await manager.IsRunningHealthyAsync(ct);

        var version = await runtimeService.GetRuntimeVersionAsync();

        if (!processIsRunning) return new GetRuntimeStatusResponse(isInstalled, version, false, null);


        var isRunning = await runtimeClient.RespondsHealthyAsync(ct);


        Uri? endpoint = null;

        if (isRunning) endpoint = new Uri(profile.Url!);

        return new GetRuntimeStatusResponse(isInstalled, version, isRunning, endpoint);
    }
}