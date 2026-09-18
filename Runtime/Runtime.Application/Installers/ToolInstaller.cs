using System.Reflection;
using DiServiceInstaller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Runtime.Application.Common.Abstractions;

namespace Runtime.Application.Installers;

public class ToolInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        List<Assembly> assemblies = [typeof(ToolInstaller).Assembly];
        var services = builder.Services;

        var toolTypes = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x =>
                x is { IsClass: true, IsAbstract: false } &&
                typeof(ITool).IsAssignableFrom(x));

        foreach (var toolType in toolTypes) services.AddScoped(typeof(ITool), toolType);
    }
}