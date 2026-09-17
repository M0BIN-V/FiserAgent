using System.ComponentModel;

namespace Runtime.WebApi.Tools;

public static class SystemSoundTools
{
    [Description("Returns current system sound volume")]
    public static async Task<double> GetCurrentSystemVolume()
    {
        var volume = new SystemVolumeService();

        return await volume.GetVolumeAsync();
    }


    [Description("Sets system sound volume")]
    public static async Task SetVolume([Description("volume")] double volume)
    {
        var volumeService = new SystemVolumeService();
        await volumeService.SetVolumeAsync(volume);
    }

    [Description("Mute or unmute sound")]
    public static async Task SetMute([Description("is muted")] bool isMuted)
    {
        var volumeService = new SystemVolumeService();
        await volumeService.SetMuteAsync(isMuted);
    }
}