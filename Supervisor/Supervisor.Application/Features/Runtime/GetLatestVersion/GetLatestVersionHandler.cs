namespace Supervisor.Application.Features.Runtime.GetLatestVersion;

public record GetLatestVersionRequest;

public record GetLatestVersionResponse(Version Version);

public class GetLatestVersionHandler(IRuntimeRegistry registry)
    : Handler<GetLatestVersionRequest, GetLatestVersionResponse>
{
    public override async Task<GetLatestVersionResponse> HandleAsync(GetLatestVersionRequest request,
        CancellationToken ct = default)
    {
        var latestVersion = await registry.GetLatestRuntimeVersionAsync();
        return new GetLatestVersionResponse(latestVersion);
    }
}