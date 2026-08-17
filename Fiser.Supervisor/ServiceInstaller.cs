using Cocona.Builder;
using Fiser.Supervisor.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Fiser.Supervisor;

public static class ServiceInstaller
{
    public static CoconaAppBuilder InstallServices(this CoconaAppBuilder builder)
    {
        var services = builder.Services;

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