using Cocona.Builder;
using Fiser.Supervisor.Common;
using Fiser.Supervisor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fiser.Supervisor;

public static class ServiceInstaller
{
    public static CoconaAppBuilder InstallServices(this CoconaAppBuilder builder)
    {
        var services = builder.Services;

        services.AddLogging(logging => { logging.AddFilter("System.Net.Http.HttpClient", LogLevel.None); });

        services.AddCommands(typeof(ServiceInstaller).Assembly);

        services.AddSingleton<RuntimePipeClient>();
        services.AddScoped<IRuntimeService, RuntimeService>();
        services.AddScoped<RuntimeProcessManager>();
        services.AddHttpClient<RuntimeProcessManager>(client => client.Timeout = TimeSpan.FromMinutes(5));
#if DEBUG
        services.AddScoped<IRuntimeRegistry, DebugRuntimeRegistry>();
#endif
        services.AddScoped<IRuntimeProcessProfileService, RuntimeProcessProfileService>();

        return builder;
    }
}