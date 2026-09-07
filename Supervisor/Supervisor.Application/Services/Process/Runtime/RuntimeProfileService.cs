using Supervisor.Application.Common.Settings;

namespace Supervisor.Application.Services.Process.Runtime;

public class RuntimeProfileService(RuntimeSettings settings) : ProfileService<RuntimeProcessProfile>
{
    protected override string ProfileFilePath { get; } = settings.ProcessProfile;
}