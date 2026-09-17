using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace Runtime.WebApi.Tools;

public sealed class SystemVolumeService
{
    private readonly CoreAudioController _controller;

    public SystemVolumeService()
    {
        _controller = new CoreAudioController();
    }

    public async Task<double> GetVolumeAsync(
        CancellationToken cancellationToken = default)
    {
        var device = await GetDefaultDeviceAsync(cancellationToken);

        return device.Volume;
    }

    public async Task<double> SetVolumeAsync(
        double volume,
        CancellationToken cancellationToken = default)
    {
        volume = Math.Clamp(volume, 0, 100);

        var device = await GetDefaultDeviceAsync(cancellationToken);

        return device.Volume = volume;
    }

    public async Task<double> IncreaseAsync(
        double amount = 5,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        var current = await GetVolumeAsync(cancellationToken);

        return await SetVolumeAsync(
            current + amount,
            cancellationToken);
    }

    public async Task<double> DecreaseAsync(
        double amount = 5,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        var current = await GetVolumeAsync(cancellationToken);

        return await SetVolumeAsync(
            current - amount,
            cancellationToken);
    }

    public async Task<bool> IsMutedAsync(
        CancellationToken cancellationToken = default)
    {
        var device = await GetDefaultDeviceAsync(cancellationToken);

        return device.IsMuted;
    }

    public async Task SetMuteAsync(
        bool muted,
        CancellationToken cancellationToken = default)
    {
        var device = await GetDefaultDeviceAsync(cancellationToken);

        await device.MuteAsync(muted);
    }


    private async Task<CoreAudioDevice> GetDefaultDeviceAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _controller.GetDefaultDeviceAsync(
            DeviceType.Playback,
            Role.Multimedia);
    }
}