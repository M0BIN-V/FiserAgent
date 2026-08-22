using Supervisor.Application.Common.Options;

namespace Supervisor.Infra.Helpers;

public static class Extensions
{
    extension(SupervisorOptions options)
    {
        public string SupervisorProjectPath => Path.Combine(options.Directory, "..", "..", "..");
    }
}