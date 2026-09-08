using System.Diagnostics;
using Supervisor.Application.Common.Contracts.Process;
using Supervisor.Application.Common.Settings;
using Supervisor.Application.Services.Process.Interface;
using Supervisor.Application.Services.Process.Runtime;

namespace Supervisor.Application.Features.Interfaces.Start;

public record StartInterfacesRequest(string interfaceUniqueName);

public record InterfaceStarted;

[GenerateOneOf]
public partial class StartInterfacesResponse : OneOfBase<
    InterfaceStarted,
    InterfaceNotFoundError,
    InterfaceIsAlreadyRunningError,
    RuntimeIsNotRunningError>;

public class StartInterfacesHandler(
    InterfacesSettings interfaceSettings,
    IProcessManagerFactory processManagerFactory,
    ILogger<StartInterfacesHandler> logger,
    RuntimeProfileService runtimeProfileService,
    InterfaceProfileServiceFactory profileServiceFactory,
    RuntimeClient runtimeClient,
    IInterfaceService interfaceService) :
    Handler<StartInterfacesRequest, StartInterfacesResponse>
{
    public override async Task<StartInterfacesResponse> HandleAsync(StartInterfacesRequest request,
        CancellationToken ct = default)
    {
        logger.LogDebug("finding interface");

        var interfaceManifest = await interfaceService.GetByUniqueNameAsync(request.interfaceUniqueName, ct);

        if (interfaceManifest is null) return new InterfaceNotFoundError(request.interfaceUniqueName);

        logger.LogDebug("validating runtime process");

        if (!await runtimeClient.RespondsHealthyAsync(ct)) return new RuntimeIsNotRunningError();

        var runtimeProfile = await runtimeProfileService.GetProfileAsync(ct);


        var interfaceProfileService = profileServiceFactory.Create(request.interfaceUniqueName);

        InterfaceProcessProfile interfaceProfile;

        logger.LogDebug("checking interface process profile");

        if (interfaceProfileService.ProfileExists())
            interfaceProfile = await interfaceProfileService.GetProfileAsync(ct);
        else
            interfaceProfile = new InterfaceProcessProfile
            {
                PipeName = Guid.CreateVersion7().ToString("N"),
                ProcessId = null,
                ProcessName = null
            };


        logger.LogDebug("checking interface process");
        var processManager = processManagerFactory.Create(interfaceProfile);

        if (await processManager.IsRunningHealthyAsync(ct))
            return new InterfaceIsAlreadyRunningError(request.interfaceUniqueName);

        logger.LogDebug("launching interface process");

        var installationPath = interfaceSettings
            .GenerateInstallationPath(request.interfaceUniqueName);


        var exeFileName = OperatingSystem.IsWindows()
            ? interfaceManifest.BinaryFileName + ".exe"
            : interfaceManifest.BinaryFileName;

        var interfaceBinaryFilePath = Path.Combine(installationPath, exeFileName);

        var env = new Dictionary<string, string>
        {
            ["OTEL_EXPORTER_OTLP_TRACES_PROTOCOL"] = "grpc",
            ["OTEL_EXPORTER_OTLP_ENDPOINT"] = "http://localhost:4317",
            ["RUNTIME_ENDPOINT"] = runtimeProfile.Url ?? throw new Exception("runtime endpint is null")
        };

        await processManager.StartProcess(
            interfaceBinaryFilePath,
            env,
            onOutput: OnOutput,
            onError: OnError,
            ct: ct);

        var timeout = new CancellationTokenSource();
        timeout.CancelAfter(TimeSpan.FromSeconds(5));

        while (!await processManager.IsRunningHealthyAsync(ct) && !timeout.Token.IsCancellationRequested)
        {
        }

        return new InterfaceStarted();
    }

    private void OnError(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is not null)
            logger.LogError(e.Data);
    }

    private void OnOutput(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is not null)
            logger.LogDebug(e.Data);
    }
}