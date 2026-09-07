using Supervisor.Application.Common.Contracts.Process;
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
    ILogger<StartInterfacesHandler> logger,
    RuntimeProfileService runtimeProfileService,
    IProcessManagerFactory managerFactory,
    InterfaceProfileServiceFactory profileServiceFactory,
    RuntimeClient runtimeClient,
    IInterfaceService interfaceService) :
    Handler<StartInterfacesRequest, StartInterfacesResponse>
{
    public override async Task<StartInterfacesResponse> HandleAsync(StartInterfacesRequest request,
        CancellationToken ct = default)
    {
        logger.LogDebug("finding interface");

        var @interface = await interfaceService.GetByUniqueNameAsync(request.interfaceUniqueName);

        if (@interface is null) return new InterfaceNotFoundError(request.interfaceUniqueName);

        logger.LogDebug("validating runtime process");

        if (!await runtimeClient.RespondsHealthyAsync(ct)) return new RuntimeIsNotRunningError();

        var profile = runtimeProfileService.GetProfileAsync(ct);

        throw new NotImplementedException();

        // var profileService = profileServiceFactory.Create();
        // var interfaceManager = managerFactory
    }
}