using DiServiceInstaller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Runtime.Application.Common.Services;
using Runtime.Application.Services;
using Runtime.Application.Tools;

namespace Runtime.Application.Installers;

public class ServicesInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        builder.Services
            .AddScoped<WindowsMediaService>()
            .AddScoped<SystemVolumeService>()
            .AddScoped<SystemBrightnessService>();
    }
}