using System.Text.Json;
using Interfaces.Sdk;
using Interfaces.Sdk.Extensions;
using Interfaces.TelegramBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TeleFrame.ApplicationBuilder;
using TeleFrame.Middlewares;
using TeleFrame.Results;
using TeleFrame.Services;
using TeleFrame.UpdateHandlers.MessageHandlers;
using TeleFrame.UpdateHandlers.MessageHandlers.CommandHandlers;
using Telegram.Bot.Types.Enums;

if (args.Any(a => a.Trim().Equals("--configure")))
{
    Console.Write("Enter your bot token: ");
    var token = Console.ReadLine();

    var config = new
    {
        Token = token
    };

    var jsonConfig = JsonSerializer.Serialize(config);

    var configPath = Path.Combine(AppContext.BaseDirectory, "botConfig.json");

    await File.WriteAllTextAsync(configPath, jsonConfig);

    return;
}

var builder = new TelegramBotBuilder(args);


builder.AddServiceDefaults();

builder.Services.AddUpdateLogging();

builder.Services.AddInterfacePipeService();

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseTracing();

app.UseUpdateLogging();

app.MapCommand("/start", () => "hi , this is fiser");


app.MapMessage(MessageType.Text, async (
    UpdateContext ctx,
    IConfiguration config,
    IHttpClientFactory factory) =>
{
    var runtimeEndpoint = config["RUNTIME_ENDPOINT"] ??
                          throw new NullReferenceException("RUNTIME_ENDPOINT");

    var httpClient = factory.CreateClient();
    httpClient.BaseAddress = new Uri(runtimeEndpoint);

    var runtimeClient = new RuntimeClient(httpClient);

    var message = ctx.Update.Message!.Text;
    var result = await runtimeClient.CompletionAsync(message);

    var r = Results.Reply(result);

    await r.InvokeAsync(ctx);
});

app.Run();