using System.ClientModel;
using DiServiceInstaller;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;
using OpenAI.Chat;

namespace Runtime.Application.Installers;

public class ChatClientServiceInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        var modelName = builder.Configuration["MODELNAME"];
        var apiKey = builder.Configuration["APIKEY"];
        var endpoint = builder.Configuration["ENDPOINT"];

        var chatClient = new ChatClient(
            modelName,
            new ApiKeyCredential(apiKey!),
            new OpenAIClientOptions { Endpoint = new Uri(endpoint!) });

        builder.Services.AddChatClient(chatClient.AsIChatClient());
    }
}