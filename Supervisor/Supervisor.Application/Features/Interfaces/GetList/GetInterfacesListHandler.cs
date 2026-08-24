namespace Supervisor.Application.Features.Interfaces.GetList;

public record GetInterfacesRequest;

public record ViewInterface(string UniqueName, string Name, Version version);

public record GetInterfacesResponse(List<ViewInterface> Interfaces);

public class GetInterfacesListHandler(
    IInterfaceRegistry registry,
    IRuntimeService runtimeService) :
    Handler<GetInterfacesRequest, GetInterfacesResponse>
{
    public override async Task<GetInterfacesResponse> HandleAsync(GetInterfacesRequest request,
        CancellationToken ct = default)
    {
        var runtimeVersion = await runtimeService.GetRuntimeVersionAsync();

        if (runtimeVersion is null) throw new InvalidOperationException("Runtime version is not available.");

        var interfaces = await registry.GetInterfaces(runtimeVersion, ct);

        var viewInterfaces = interfaces
            .Select(i => new ViewInterface(i.UniqueName, i.Name, i.Version))
            .ToList();

        return new GetInterfacesResponse(viewInterfaces);
    }
}