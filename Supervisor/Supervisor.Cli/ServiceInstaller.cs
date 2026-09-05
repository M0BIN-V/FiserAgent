using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Supervisor.Application.Common.Extensions;
using Supervisor.Application.Features.Runtime.Shutdown;
using Supervisor.Application.Services;
using Supervisor.Application.Services.ProcessProfile;
using Supervisor.Infra.Services;

namespace Supervisor.Cli;

public static class ServiceInstaller
{
    public static CoconaAppBuilder InstallServices(this CoconaAppBuilder builder)
    {
        var services = builder.Services;

        services.AddLogging(logging => { logging.AddFilter("System.Net.Http.HttpClient", LogLevel.None); });

        services.AddCommands(typeof(ServiceInstaller).Assembly);

        services.RegisterHandlers(typeof(ShutdownRuntimeHandler).Assembly);

        services.AddScoped<ShutdownRuntimeHandler>();

        services.AddScoped<ProfileService<RuntimeProcessProfile>, RuntimeProfileService>();

        services.AddScoped<InterfaceProfileServiceFactory>();

        services.AddSingleton<PipeClient>();
        services.AddScoped<RuntimeProfileService>();
        services.AddScoped<ProcessManagerFactory>();
        services.AddScoped<IRuntimeService, RuntimeService>();
        services.AddScoped<RuntimeClient>();
        services.AddHttpClient<RuntimeClient>(client => client.Timeout = TimeSpan.FromMinutes(5));
#if DEBUG
        services.AddScoped<IRuntimeRegistry, DebugRuntimeRegistry>();
        services.AddScoped<IInterfaceRegistry, DebugInterfaceRegistry>();
#endif
        services.AddScoped<IRuntimeProcessProfileService, RuntimeProcessProfileService>();

        return builder;
    }
}