using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Supervisor.Application.Common.Contracts.Process;
using Supervisor.Application.Common.Extensions;
using Supervisor.Application.Common.Settings;
using Supervisor.Application.Features.Runtime.Shutdown;
using Supervisor.Application.Services;
using Supervisor.Application.Services.Process;
using Supervisor.Application.Services.Process.Interface;
using Supervisor.Application.Services.Process.Runtime;
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

        services.AddScoped<ProfileService<RuntimeProcessProfile>, RuntimeProfileService>();

        services.AddScoped<InterfaceProfileServiceFactory>();

        services.AddSingleton<RuntimeSettings>();
        services.AddSingleton<InterfacesSettings>();
        services.AddSingleton<SupervisorSettings>();

        services.AddSingleton<IPipeClient, PipeClient>();
        services.AddScoped<RuntimeProfileService>();
        services.AddScoped<IProcessManagerFactory, ProcessManagerFactory>();
        services.AddScoped<IRuntimeService, RuntimeService>();
        services.AddScoped<RuntimeClient>();
        services.AddHttpClient<RuntimeClient>(client => client.Timeout = TimeSpan.FromMinutes(5));
#if DEBUG
        services.AddScoped<IRuntimeRegistry, DebugRuntimeRegistry>();
        services.AddScoped<IInterfaceRegistry, DebugInterfaceRegistry>();
#endif

        return builder;
    }
}