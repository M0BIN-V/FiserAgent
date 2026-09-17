using TeleFrame.Services;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace Interfaces.TelegramBot;

internal sealed class DraftUpdater
{
    private readonly long _chatId;
    private readonly UpdateContext _ctx;
    private readonly int _draftId;
    private readonly TimeSpan _interval;
    private DateTimeOffset _lastSentAt = DateTimeOffset.MinValue;
    private string? _pendingText;

    public DraftUpdater(UpdateContext ctx, long chatId, int draftId, TimeSpan interval)
    {
        _ctx = ctx;
        _chatId = chatId;
        _draftId = draftId;
        _interval = interval;
    }

    public async Task UpdateAsync(string text, CancellationToken cancellationToken)
    {
        // Telegram rejects empty messages.
        if (string.IsNullOrWhiteSpace(text)) return;
        _pendingText = text;
        var elapsed = DateTimeOffset.UtcNow - _lastSentAt;
        if (elapsed < _interval) return;
        await FlushAsync(cancellationToken);
    }

    public async Task FlushAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_pendingText)) return;
        var text = _pendingText;
        _pendingText = null;
        await DraftSender.SendDraftWithRetryAsync(_ctx, _chatId, _draftId, text, cancellationToken);
        _lastSentAt = DateTimeOffset.UtcNow;
    }
}

public static class DraftSender
{
    public static async Task SendDraftWithRetryAsync(UpdateContext ctx, long chatId, int draftId, string text,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        const int maxAttempts = 4;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
            try
            {
                await ctx.Client.SendMessageDraft(chatId, draftId, text, ParseMode.MarkdownV2,
                    cancellationToken: cancellationToken);
                return;
            }
            catch (ApiRequestException ex) when (ex.ErrorCode == 429 && attempt < maxAttempts)
            {
                // Telegram.Bot exposes RetryAfter for rate-limit responses // in supported versions.
                var retryAfter = ex.Parameters?.RetryAfter ?? 1;
                await Task.Delay(TimeSpan.FromSeconds(retryAfter), cancellationToken);
            }
    }

    public static async Task SendMessageWithRetryAsync(UpdateContext ctx, long chatId, string text,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        const int maxAttempts = 4;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
            try
            {
                await ctx.Client.SendMessage(chatId, text, ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                return;
            }
            catch (ApiRequestException ex) when (ex.ErrorCode == 429 && attempt < maxAttempts)
            {
                var retryAfter = ex.Parameters?.RetryAfter ?? 1;
                await Task.Delay(TimeSpan.FromSeconds(retryAfter), cancellationToken);
            }
    }
}