using System.Net.Http.Json;
using System.Net.ServerSentEvents;
using System.Text.Json;
using System.Text.Json.Serialization;
using Interfaces.Sdk;
using Interfaces.Sdk.Extensions;
using Interfaces.TelegramBot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeleFrame.ApplicationBuilder;
using TeleFrame.Middlewares;
using TeleFrame.Services;
using TeleFrame.UpdateHandlers.MessageHandlers;
using TeleFrame.UpdateHandlers.MessageHandlers.CommandHandlers;
using Telegram.Bot;
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
builder.Services.AddRuntimeHttpClient(builder.Configuration);
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
    ILogger<CompletionRequest> logger,
    UpdateContext ctx,
    IHttpClientFactory clientFactory,
    CancellationToken cancellationToken) =>
{
    logger.LogInformation("initializing chat completion");

    var message = ctx.Update.Message;
    var chatId = message!.Chat.Id;
    var draftId = Random.Shared.Next(1, int.MaxValue);

    var httpClient = clientFactory.CreateClient(ServiceCollectionExtensions.RuntimeHttpClientName);
    var request = new HttpRequestMessage(HttpMethod.Post, "/completion")
    {
        Content = JsonContent.Create(new CompletionRequest
        {
            Text = message.Text
        })
    };

    await DraftSender.SendDraftWithRetryAsync(ctx, chatId, draftId, "⌬ Thinking", cancellationToken);

    var renderer = new TelegramMarkdownV2Renderer();


    logger.LogInformation("sending request to runtime");
    using var response = await httpClient
        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
    response.EnsureSuccessStatusCode();

    logger.LogInformation("reading response stream");

    var jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    jsonOptions.Converters.Add(new JsonStringEnumConverter());

    await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
    var parser = SseParser.Create(stream,
        (_, data) => JsonSerializer.Deserialize<CompletionResponse>(data, jsonOptions)!);
    var fullText = string.Empty;

    var draftUpdater = new DraftUpdater(ctx, chatId, draftId, TimeSpan.FromMilliseconds(400));

    logger.LogInformation("reading event items");
    await foreach (var item in parser.EnumerateAsync(cancellationToken))
    {
        var completion = item.Data;

        switch (completion.Type)
        {
            case ChatEventType.ToolCall:
            {
                logger.LogInformation("tool call event received");

                if (string.IsNullOrWhiteSpace(completion.ToolViewName))
                    continue;

                var toolName =
                    TelegramMarkdownV2Renderer.RenderInlineCode(
                        completion.ToolViewName);

                var toolText = $"⌬ Calling {toolName}\\.\\.\\.";

                await draftUpdater.FlushAsync(cancellationToken);

                await DraftSender.SendDraftWithRetryAsync(
                    ctx,
                    chatId,
                    draftId,
                    toolText,
                    cancellationToken);

                break;
            }
            case ChatEventType.Text:
            {
                logger.LogInformation($"text event received : {completion.Text}");
                if (string.IsNullOrEmpty(completion.Text)) continue;
                fullText += completion.Text;
                var renderedText = renderer.Render(fullText);
                if (string.IsNullOrWhiteSpace(renderedText)) continue;
                await draftUpdater.UpdateAsync(renderedText, cancellationToken);
                break;
            }
            case ChatEventType.Completed:
            {
                logger.LogInformation("completed event received");
                await draftUpdater.FlushAsync(cancellationToken);
                break;
            }
            case ChatEventType.ToolResult:
            {
                logger.LogInformation("tool result event received");
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    if (!string.IsNullOrWhiteSpace(fullText))
    {
        var finalText = renderer.Render(fullText);
        if (!string.IsNullOrWhiteSpace(finalText))
            await DraftSender.SendMessageWithRetryAsync(ctx, chatId, finalText, cancellationToken);
    }
});

app.Run();