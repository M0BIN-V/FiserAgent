using System.Management;

namespace Runtime.WebApi.Tools;

public sealed class SystemBrightnessService
{
    private const string WmiNamespace = @"root\WMI";

    public async Task<int> GetBrightnessAsync(
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var searcher = new ManagementObjectSearcher(
                WmiNamespace,
                "SELECT CurrentBrightness FROM WmiMonitorBrightness WHERE Active = TRUE");

            using var results = searcher.Get();

            foreach (ManagementObject monitor in results)
                using (monitor)
                {
                    return Convert.ToInt32(
                        monitor["CurrentBrightness"]);
                }

            throw new InvalidOperationException(
                "No active monitor with software brightness control was found.");
        }, cancellationToken);
    }

    public async Task<int> SetBrightnessAsync(
        int brightness,
        CancellationToken cancellationToken = default)
    {
        brightness = Math.Clamp(brightness, 0, 100);

        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var searcher = new ManagementObjectSearcher(
                WmiNamespace,
                "SELECT * FROM WmiMonitorBrightnessMethods");

            using var results = searcher.Get();

            var found = false;

            foreach (ManagementObject monitor in results)
                using (monitor)
                {
                    found = true;

                    var result = monitor.InvokeMethod(
                        "WmiSetBrightness",
                        new object[]
                        {
                            0,
                            (byte)brightness
                        });

                    var returnCode = Convert.ToUInt32(result);

                    if (returnCode != 0)
                        throw new InvalidOperationException(
                            $"Failed to set screen brightness. WMI error code: {returnCode}");
                }

            if (!found)
                throw new InvalidOperationException(
                    "No monitor with software brightness control was found.");

            return brightness;
        }, cancellationToken);
    }

    public async Task<int> IncreaseAsync(
        int amount = 5,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        var current = await GetBrightnessAsync(cancellationToken);

        return await SetBrightnessAsync(
            current + amount,
            cancellationToken);
    }

    public async Task<int> DecreaseAsync(
        int amount = 5,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        var current = await GetBrightnessAsync(cancellationToken);

        return await SetBrightnessAsync(
            current - amount,
            cancellationToken);
    }
}