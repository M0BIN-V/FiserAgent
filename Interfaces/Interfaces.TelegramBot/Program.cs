using System.Text.Json;
using Interfaces.Sdk;
using Interfaces.Sdk.Extensions;
using Interfaces.TelegramBot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TeleFrame.ApplicationBuilder;
using TeleFrame.Middlewares;
using TeleFrame.Services;
using TeleFrame.UpdateHandlers.MessageHandlers;
using TeleFrame.UpdateHandlers.MessageHandlers.CommandHandlers;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
builder.Services.AddRuntimeClient(builder.Configuration);

var app = builder.Build();

app.UseTracing();

app.UseUpdateLogging();

app.MapCommand("/start", async (UpdateContext context) =>
{
    var botClient = context.Client;

    var chatId = context.Update.Message!.Chat.Id;

    await botClient.SendMessage(
        chatId,
        "👋 Hello! I'm your friendly AI assistant. \n Just type your message and I'll do my best to assist you! 🤖💬"
    );
});


app.MapMessage(MessageType.Text, async (
    UpdateContext ctx,
    RuntimeClient runtimeClient) =>
{
    var draftId = Random.Shared.Next(1, 9999);

    var chatId = ctx.Update.Message!.Chat.Id;
    await ctx.Client.SendMessageDraft(
        chatId,
        draftId,
        "⌬ Thinking");

    var message = ctx.Update.Message!.Text;
    var result = await runtimeClient.CompletionAsync(message);

    var text = result.Text ?? "i";

    var keyboard = new InlineKeyboardMarkup(
    [
        [
            InlineKeyboardButton.WithCallbackData($"out :{result.OutputToken}"),
            InlineKeyboardButton.WithCallbackData($"in :{result.InputToken}")
        ]
    ]);

    await ctx.Client.SendMessageDraft(
        chatId,
        draftId,
        text);

    await ctx.Client.SendMessage(chatId, text, replyMarkup: keyboard);
});

app.Run();