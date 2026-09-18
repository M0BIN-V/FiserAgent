using TeleFrame.Services;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace Interfaces.TelegramBot;

internal sealed class DraftUpdater(
    UpdateContext ctx,
    long chatId,
    int draftId,
    TimeSpan interval,
    int? threadId)
{
    private DateTimeOffset _lastSentAt = DateTimeOffset.MinValue;
    private string? _pendingText;

    public async Task UpdateAsync(string text, CancellationToken cancellationToken)
    {
        // Telegram rejects empty messages.
        if (string.IsNullOrWhiteSpace(text)) return;
        _pendingText = text;
        var elapsed = DateTimeOffset.UtcNow - _lastSentAt;
        if (elapsed < interval) return;
        await FlushAsync(cancellationToken);
    }

    public async Task FlushAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_pendingText)) return;
        var text = _pendingText;
        _pendingText = null;
        await DraftSender.SendDraftWithRetryAsync(ctx, chatId, draftId, text, threadId, cancellationToken);
        _lastSentAt = DateTimeOffset.UtcNow;
    }
}

public static class DraftSender
{
    public static async Task SendDraftWithRetryAsync(
        UpdateContext ctx,
        long chatId,
        int draftId,
        string text,
        int? threadId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        const int maxAttempts = 4;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
            try
            {
                await ctx.Client
                    .SendMessageDraft(
                        chatId,
                        draftId,
                        text,
                        ParseMode.MarkdownV2,
                        threadId,
                        cancellationToken: cancellationToken);
                return;
            }
            catch (ApiRequestException ex) when (ex.ErrorCode == 429 && attempt < maxAttempts)
            {
                var retryAfter = ex.Parameters?.RetryAfter ?? 1;
                await Task.Delay(TimeSpan.FromSeconds(retryAfter), cancellationToken);
            }
    }

    public static async Task SendMessageWithRetryAsync(UpdateContext ctx, long chatId, string text, int? threadId,
        List<string> toolCalls,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        const int maxAttempts = 4;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
            try
            {
                await ctx.Client.SendMessage(
                    chatId,
                    text,
                    ParseMode.MarkdownV2,
                    messageThreadId: threadId,
                    cancellationToken: cancellationToken);
                return;
            }
            catch (ApiRequestException ex) when (ex.ErrorCode == 429 && attempt < maxAttempts)
            {
                var retryAfter = ex.Parameters?.RetryAfter ?? 1;
                await Task.Delay(TimeSpan.FromSeconds(retryAfter), cancellationToken);
            }
    }
}