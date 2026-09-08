using Supervisor.Application.Features.Interfaces.Start;

namespace Supervisor.Application.Features.Interfaces.GetInstalledInterfaces;

public record GetInstalledInterfacesRequest;

public record InstalledInterface(string UniqueName, string Name, Version Version);

public record GetInstalledInterfacesResponse(List<InstalledInterface> Interfaces);

public class GetInstalledInterfacesHandler(
    IInterfaceService service) :
    Handler<GetInstalledInterfacesRequest, GetInstalledInterfacesResponse>
{
    public override async Task<GetInstalledInterfacesResponse> HandleAsync(GetInstalledInterfacesRequest request,
        CancellationToken ct = default)
    {
        var installed = await service.GetInstalledInterfacesAsync(ct);

        return new GetInstalledInterfacesResponse(installed
            .Select(i => new InstalledInterface(i.UniqueName, i.Name, i.Version))
            .ToList());
    }
}