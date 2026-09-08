using Supervisor.Application.Common.Settings;
using Supervisor.Application.Features.Interfaces.Start;

namespace Supervisor.Application.Features.Interfaces.Configure;

public record ConfigureInterfaceRequest(string interfaceUniqueName);

[GenerateOneOf]
public partial class ConfigureInterfaceResponse : OneOfBase<
    string,
    InterfaceNotFoundError>;

public class ConfigureInterfaceHandler(
    ForegroundProcessRunner runner,
    IInterfaceService service) : Handler<ConfigureInterfaceRequest, ConfigureInterfaceResponse>
{
    public override async Task<ConfigureInterfaceResponse> HandleAsync(
        ConfigureInterfaceRequest request, CancellationToken ct = default)
    {
        var manifest = await service.GetByUniqueNameAsync(request.interfaceUniqueName, ct);

        if (manifest is null) return new InterfaceNotFoundError(request.interfaceUniqueName);

        var binaryFileName = OperatingSystem.IsWindows() ? manifest.BinaryFileName + ".exe" : manifest.BinaryFileName;

        var interfaceDirectoryPath = await service.GetInterfaceDirectoryPathAsync(request.interfaceUniqueName, ct) ??
                                     throw new DirectoryNotFoundException(
                                         $"cant find directory for interface :{request.interfaceUniqueName}");

        var filePath = Path.Combine(interfaceDirectoryPath, binaryFileName);

        runner.Run(filePath,"--configure");

        return "started";
    }
}