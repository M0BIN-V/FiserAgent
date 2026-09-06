using Interfaces.TelegramBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TeleFrame.ApplicationBuilder;
using TeleFrame.Middlewares;
using TeleFrame.UpdateHandlers.MessageHandlers;
using TeleFrame.UpdateHandlers.MessageHandlers.CommandHandlers;
using Telegram.Bot.Types.Enums;

var builder = new TelegramBotBuilder(args);

builder.Services.AddUpdateLogging();

builder.Services.AddHostedService<InterfacePipeService>();

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseUpdateLogging();

app.MapCommand("/start", () => "hi this is fiser");

app.MapMessage(MessageType.Text, (
    IConfiguration config,
    IHttpClientFactory factory) =>
{
    var runtimeEndpoint = config["RUNTIME_ENDPOINT"] ??
                          throw new NullReferenceException("RUNTIME_ENDPOINT");

    var client = factory.CreateClient();
    client.BaseAddress = new Uri(runtimeEndpoint);
});

app.Run();