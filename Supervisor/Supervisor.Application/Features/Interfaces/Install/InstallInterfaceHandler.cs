using Supervisor.Application.Common.Errors;

namespace Supervisor.Application.Features.Interfaces.Install;

public record InstallInterfaceRequest(
    string UniqueName,
    Version Version,
    IProgress<ProgressUpdate>? progress = null);

public class InstallInterfaceHandler(
    IInterfaceProcessManager interfaceProcessManager,
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

        if (await interfaceProcessManager.InterfaceIsRunningAsync(request.UniqueName, ct))
            await interfaceProcessManager.ShutdownInterfaceAsync(ct);

        await registry.FetchAsync(request.UniqueName, @interface.Version, request.progress);

        return @interface.Version;
    }
}

[GenerateOneOf]
public partial class InstallInterfaceResponse : OneOfBase<
    Version,
    InterfaceNotFoundError,
    RuntimeIsNotInstalledError>;