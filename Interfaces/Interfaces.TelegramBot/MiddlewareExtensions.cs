using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using TeleFrame.Application;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Interfaces.TelegramBot;

public static class MiddlewareExtensions
{
    extension(TelegramBotApplication app)
    {
        public TelegramBotApplication UseTracing()
        {
            return app.Use(next => async (context, ct) =>
            {
                var tracerProvider = context.Services.GetRequiredService<TracerProvider>();
                var tracer = tracerProvider.GetTracer("TelegramBot");
                using var span = tracer.StartActiveSpan("Update");

                if (context.Update.Type == UpdateType.Message)
                    _ = context.Client.SendChatAction(
                        context.Update.Message!.Chat,
                        ChatAction.Typing,
                        cancellationToken: ct);

                await next(context, ct);
            });
        }
    }
}