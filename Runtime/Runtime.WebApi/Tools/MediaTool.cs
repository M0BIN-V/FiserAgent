using System.ComponentModel;

namespace Runtime.WebApi.Tools;

public static class MediaTools
{
    [Description("Pauses the currently playing music or video on the computer.")]
    public static async Task<string> PauseMedia()
    {
        var success = await new WindowsMediaController().PauseAsync();

        return success
            ? "Media paused."
            : "There is no active media session or the media could not be paused.";
    }

    [Description("Starts or resumes the currently paused music or video on the computer.")]
    public static async Task<string> PlayMedia()
    {
        var success = await new WindowsMediaController().PlayAsync();

        return success
            ? "Media started."
            : "There is no active media session or the media could not be started.";
    }

    [Description("Toggles the currently playing media between play and pause.")]
    public static async Task<string> ToggleMedia()
    {
        var success = await new WindowsMediaController().ToggleAsync();

        return success
            ? "Media playback toggled."
            : "There is no active media session or the media state could not be changed.";
    }

    [Description("Skips to the next track or media item in the currently active media session.")]
    public static async Task<string> NextMedia()
    {
        var success = await new WindowsMediaController().NextAsync();

        return success
            ? "Skipped to the next media item."
            : "The active media session does not support skipping to the next item.";
    }

    [Description("Goes back to the previous track or media item in the currently active media session.")]
    public static async Task<string> PreviousMedia()
    {
        var success = await new WindowsMediaController().PreviousAsync();

        return success
            ? "Moved to the previous media item."
            : "The active media session does not support going to the previous item.";
    }

    [Description(
        "Gets information about the currently active music or video, including title, artist, album and playback state.")]
    public static async Task<MediaStatus?> GetMediaStatus()
    {
        return await new WindowsMediaController().GetStatusAsync();
    }
}