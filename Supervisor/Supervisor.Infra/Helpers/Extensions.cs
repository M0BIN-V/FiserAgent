using Supervisor.Application.Common.Settings;

namespace Supervisor.Infra.Helpers;

public static class Extensions
{
    extension(SupervisorSettings options)
    {
        public string SupervisorProjectPath => Path.Combine(options.InstallationDirectory, "..", "..", "..");
    }
}