using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Runtime.Application.Common.Abstractions;

namespace Runtime.Application.Common.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddTools(this IServiceCollection services, params Assembly[] assemblies)
    {
        var toolTypes = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => x is { IsClass: true, IsAbstract: false } &&
                        typeof(ITool).IsAssignableFrom(x));

        foreach (var toolType in toolTypes) services.AddScoped(typeof(ITool), toolType);

        return services;
    }
}