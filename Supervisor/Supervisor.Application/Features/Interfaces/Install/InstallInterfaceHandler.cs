using Supervisor.Application.Common.Errors;
using Supervisor.Application.Services.Process;
using Supervisor.Application.Services.Process.Interface;

namespace Supervisor.Application.Features.Interfaces.Install;

public record InstallInterfaceRequest(
    string UniqueName,
    Version Version,
    IProgress<ProgressUpdate>? progress = null);

public class InstallInterfaceHandler(
    ProcessManagerFactory managerFactory,
    InterfaceProfileServiceFactory profileServiceFactory,
    IRuntimeService runtimeService,
    IInterfaceRegistry registry)
    : Handler<InstallInterfaceRequest, InstallInterfaceResponse>
{
    public override async Task<InstallInterfaceResponse> HandleAsync(InstallInterfaceRequest request,
        CancellationToken ct = default)
    {
        if (!runtimeService.RunIsTimeInstalled()) return new RuntimeIsNotInstalledError();

        var runtimeVersion = await runtimeService.GetRuntimeVersionAsync();

        var @interface = await registry.GetAsync(request.UniqueName, request.Version, runtimeVersion!, ct);

        if (@interface is null) return new InterfaceNotFoundError(request.UniqueName);

        var profileService = profileServiceFactory.Create(@interface.UniqueName);
        var profile = await profileService.GetProfileAsync(ct);
        var manager = managerFactory.Create(profile);

        if (await manager.IsRunningHealthyAsync(ct))
            await manager.ShutdownAsync(ct);

        await registry.FetchAsync(request.UniqueName, @interface.Version, request.progress);

        return @interface.Version;
    }
}

[GenerateOneOf]
public partial class InstallInterfaceResponse : OneOfBase<
    Version,
    InterfaceNotFoundError,
    RuntimeIsNotInstalledError>;