using System.ComponentModel;
using Runtime.Application.Common.Abstractions;
using Runtime.Application.Services;

namespace Runtime.Application.Tools;

public class SystemSoundTools(SystemVolumeService volumeService) : ITool
{
    [Description("Returns current system sound volume")]
    public Task<double> GetCurrentSystemVolume()
    {
        return volumeService.GetVolumeAsync();
    }

    [Description("Sets system sound volume")]
    public Task SetVolume([Description("volume")] double volume)
    {
        return volumeService.SetVolumeAsync(volume);
    }

    [Description("Mute or unmute sound")]
    public Task SetMute([Description("is muted")] bool isMuted)
    {
        return volumeService.SetMuteAsync(isMuted);
    }
}