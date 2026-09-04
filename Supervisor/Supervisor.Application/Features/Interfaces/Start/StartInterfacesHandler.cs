using Supervisor.Application.Common.Errors;
using Supervisor.Application.Services;
using Supervisor.Domain.Entities;

namespace Supervisor.Application.Features.Interfaces.Start;

public record StartInterfacesRequest(string interfaceUniqueName);

public record InterfaceStarted;

[GenerateOneOf]
public partial class StartInterfacesResponse : OneOfBase<
    InterfaceStarted,
    InterfaceNotFoundError,
    InterfaceIsAlreadyRunningError>;

public class StartInterfacesHandler(
    InterfaceProcessManager processManager,
    IInterfaceService interfaceService) :
    Handler<StartInterfacesRequest, StartInterfacesResponse>
{
    public override async Task<StartInterfacesResponse> HandleAsync(StartInterfacesRequest request,
        CancellationToken ct = default)
    {
        var @interface = await interfaceService.GetByUniqueNameAsync(request.interfaceUniqueName);

        if (@interface is null) return new InterfaceNotFoundError(request.interfaceUniqueName);

        // if (await processManager.IsRunningHealthyAsync(request.interfaceUniqueName, ct))
        //     return new InterfaceIsAlreadyRunningError(request.interfaceUniqueName);
        //
        //  await processManager.(request.interfaceUniqueName);
        
        throw new NotImplementedException("Starting interfaces is not implemented yet.");
    }
}

public interface IInterfaceService
{
    public List<Interface> GetInstalledInterfaces();
    public bool IsInterfaceInstalled(string interfaceUniqueName);
    public Task<Interface?> GetByUniqueNameAsync(string interfaceUniqueName);
}