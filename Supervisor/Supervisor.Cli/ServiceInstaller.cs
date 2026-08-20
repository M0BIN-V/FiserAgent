using Cocona.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Supervisor.Application.Contracts;
using Supervisor.Application.Features.Shutdown;
using Supervisor.Application.Services;
using Supervisor.Cli.Application.Common;
using Supervisor.Infra.Services;

namespace Supervisor.Cli;

public static class ServiceInstaller
{
    public static CoconaAppBuilder InstallServices(this CoconaAppBuilder builder)
    {
        var services = builder.Services;

        services.AddLogging(logging => { logging.AddFilter("System.Net.Http.HttpClient", LogLevel.None); });

        services.AddCommands(typeof(ServiceInstaller).Assembly);

        services.AddScoped<ShutdownHandler>();

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