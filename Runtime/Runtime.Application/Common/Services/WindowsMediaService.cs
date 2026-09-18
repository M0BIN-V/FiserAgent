using Windows.Media.Control;
using Runtime.Application.Common.Abstractions;

namespace Runtime.Application.Common.Services;

public class WindowsMediaService : ITool
{
    private async Task<GlobalSystemMediaTransportControlsSession?> GetCurrentSessionAsync()
    {
        var manager =
            await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

        return manager.GetCurrentSession();
    }

    public async Task<bool> PlayAsync()
    {
        var session = await GetCurrentSessionAsync();

        if (session is null)
            return false;

        return await session.TryPlayAsync();
    }

    public async Task<bool> PauseAsync()
    {
        var session = await GetCurrentSessionAsync();

        if (session is null)
            return false;

        return await session.TryPauseAsync();
    }

    public async Task<bool> ToggleAsync()
    {
        var session = await GetCurrentSessionAsync();

        if (session is null)
            return false;

        var playbackInfo = session.GetPlaybackInfo();

        return playbackInfo.PlaybackStatus switch
        {
            GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing
                => await session.TryPauseAsync(),

            GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused
                => await session.TryPlayAsync(),

            GlobalSystemMediaTransportControlsSessionPlaybackStatus.Stopped
                => await session.TryPlayAsync(),

            _ => false
        };
    }

    public async Task<bool> NextAsync()
    {
        var session = await GetCurrentSessionAsync();

        if (session is null)
            return false;

        return await session.TrySkipNextAsync();
    }

    public async Task<bool> PreviousAsync()
    {
        var session = await GetCurrentSessionAsync();

        if (session is null)
            return false;

        return await session.TrySkipPreviousAsync();
    }

    public async Task<MediaStatus?> GetStatusAsync()
    {
        var session = await GetCurrentSessionAsync();

        if (session is null)
            return null;

        var playbackInfo = session.GetPlaybackInfo();
        var properties = await session.TryGetMediaPropertiesAsync();

        return new MediaStatus(
            session.SourceAppUserModelId,
            properties?.Title,
            properties?.Artist,
            properties?.AlbumTitle,
            playbackInfo.PlaybackStatus ==
            GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing);
    }
}

public sealed record MediaStatus(
    string? AppId,
    string? Title,
    string? Artist,
    string? Album,
    bool IsPlaying);