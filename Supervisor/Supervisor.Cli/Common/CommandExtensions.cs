using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Supervisor.Cli.Application.Common;

public static class CommandExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services, params Assembly[] assemblies)
    {
        var commandTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(x => typeof(ICommand).IsAssignableFrom(x) &&
                        x is { IsClass: true, IsAbstract: false });

        foreach (var commandType in commandTypes) services.AddSingleton(typeof(ICommand), commandType);

        return services;
    }

    public static CoconaApp MapCommands(this CoconaApp app)
    {
        foreach (var command in app.Services.GetServices<ICommand>()) command.Map(app);
        return app;
    }
}