using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Interfaces.Sdk.Extensions;

public static class ServiceCollectionExtensions
{
    public const string RuntimeHttpClientName = "RuntimeHttpClient";

    public static IServiceCollection AddInterfacePipeService(this IServiceCollection services)
    {
        services.AddHostedService<InterfacePipeService>();

        return services;
    }

    public static IServiceCollection AddRuntimeHttpClient(this IServiceCollection services, IConfiguration config)
    {
        var runtimeEndpoint = config["RUNTIME_ENDPOINT"] ?? throw new NullReferenceException("RUNTIME_ENDPOINT");
        services.AddHttpClient(RuntimeHttpClientName, c =>
        {
            c.BaseAddress = new Uri(runtimeEndpoint);
            c.Timeout = TimeSpan.FromMinutes(1);
        });

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