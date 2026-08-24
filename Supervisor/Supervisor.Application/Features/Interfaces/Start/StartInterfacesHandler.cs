using System.Data;

namespace Supervisor.Application.Features.Interfaces.Start;

public record StartInterfacesRequest(string interfaceUniqueName);

public record StartInterfacesResponse;

public class StartInterfacesHandler (IInterfaceRegistry registry):
    Handler<StartInterfacesRequest, StartInterfacesResponse>
{
    public override Task<StartInterfacesResponse> HandleAsync(StartInterfacesRequest request,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}