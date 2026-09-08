using Microsoft.Extensions.DependencyInjection;

namespace Interfaces.Sdk.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInterfacePipeService(this IServiceCollection services)
    {
        services.AddHostedService<InterfacePipeService>();

        return services;
    }
}