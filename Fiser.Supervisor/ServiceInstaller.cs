using Cocona.Builder;
using Fiser.Supervisor.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Fiser.Supervisor;

public static class ServiceInstaller
{
    public static CoconaAppBuilder InstallServices(this CoconaAppBuilder builder)
    {
        var services = builder.Services;

        services.AddSingleton<IRuntimeService,DebugRuntimeService>();


        return builder;
    }
}