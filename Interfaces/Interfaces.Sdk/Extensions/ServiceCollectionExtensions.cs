using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Interfaces.Sdk.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInterfacePipeService(this IServiceCollection services)
    {
        services.AddHostedService<InterfacePipeService>();

        return services;
    }

    public static IServiceCollection AddRuntimeClient(this IServiceCollection services, IConfiguration config)
    {
        var runtimeEndpoint = config["RUNTIME_ENDPOINT"] ??
                              throw new NullReferenceException("RUNTIME_ENDPOINT");

        services.AddHttpClient<RuntimeClient>(c => c.BaseAddress = new Uri(runtimeEndpoint));

        return services;
    }
}