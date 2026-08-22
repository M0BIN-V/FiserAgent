namespace Supervisor.Application.Features.Interfaces.StartInterfaces;

public record StartInterfacesRequest(Uri RuntimeEndpoint);

public record StartInterfacesResponse;

public class StartInterfacesHandler(
    IInterfaceRegistry registry) :
    Handler<StartInterfacesRequest, StartInterfacesResponse>
{
    public override Task<StartInterfacesResponse> HandleAsync(StartInterfacesRequest request,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}