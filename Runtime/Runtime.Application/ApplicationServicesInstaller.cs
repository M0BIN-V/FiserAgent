using DiServiceInstaller;
using Microsoft.Extensions.Hosting;

namespace Runtime.Application;

public static class ApplicationServicesInstaller
{
    public static IHostApplicationBuilder InstallApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.InstallServices(typeof(ApplicationServicesInstaller).Assembly);

        return builder;
    }
}