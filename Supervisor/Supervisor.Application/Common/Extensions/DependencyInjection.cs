using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Supervisor.Application.Common.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection RegisterHandlers(this IServiceCollection services, Assembly assembly)
    {
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(Handler<>)))
            .AsSelf()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(Handler<,>)))
            .AsSelf()
            .WithScopedLifetime());

        return services;
    }
}