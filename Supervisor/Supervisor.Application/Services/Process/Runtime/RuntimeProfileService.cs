namespace Supervisor.Application.Services.Process.Runtime;

public class RuntimeProfileService(IOptions<RuntimeOptions> options) : ProfileService<RuntimeProcessProfile>
{
    protected override string ProfileFilePath { get; } = options.Value.ProcessProfile;
}