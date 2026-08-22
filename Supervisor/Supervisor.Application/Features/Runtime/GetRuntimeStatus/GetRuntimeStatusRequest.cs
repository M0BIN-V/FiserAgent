using Supervisor.Application.Services;

namespace Supervisor.Application.Features.Runtime.GetRuntimeStatus;

public record GetRuntimeStatusRequest;

public record GetRuntimeStatusResponse(
    bool Installed,
    Version? Version,
    bool IsRunning,
    Uri? Endpoint);

public class GetRuntimeStatusHandler(
    IRuntimeProcessProfileService profileService,
    IRuntimeService runtimeService,
    RuntimeProcessManager runtimeProcessManager) :
    Handler<GetRuntimeStatusRequest, GetRuntimeStatusResponse>
{
    public override async Task<GetRuntimeStatusResponse> HandleAsync(GetRuntimeStatusRequest request,
        CancellationToken ct = default)
    {
        var isInstalled = runtimeService.RunIsTimeInstalled();

        if (!isInstalled) return new GetRuntimeStatusResponse(isInstalled, null, false, null);

        var versionResult = runtimeService.GetRuntimeVersionAsync();
        var isRunningResult = runtimeProcessManager.IsRunningAsync(ct);
        
        await Task.WhenAll(versionResult, isRunningResult);

        Uri? endpoint = null;

        if (isRunningResult.Result)
        {
            var profile = await profileService.GetProfileAsync(ct);
            endpoint = new Uri(profile.Url);
        }

        return new GetRuntimeStatusResponse(
            isInstalled,
            versionResult.Result,
            isRunningResult.Result,
            endpoint);
    }
}