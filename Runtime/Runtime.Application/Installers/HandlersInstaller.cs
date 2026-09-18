using DiServiceInstaller;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Runtime.Application.Features.CompleteChat;

namespace Runtime.Application.Installers;

public class HandlersInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(typeof(HandlersInstaller).Assembly);

        builder.Services.AddScoped<CompleteChatHandler>();
    }
}