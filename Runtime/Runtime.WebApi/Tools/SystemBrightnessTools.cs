using System.ComponentModel;

namespace Runtime.WebApi.Tools;

public static class SystemBrightnessTools
{
    [Description("Returns current screen brightness percentage")]
    public static async Task<int> GetCurrentScreenBrightness()
    {
        var brightness = new SystemBrightnessService();

        return await brightness.GetBrightnessAsync();
    }

    [Description("Sets screen brightness percentage")]
    public static async Task SetBrightness(
        [Description("brightness percentage from 0 to 100")]
        int brightness)
    {
        var brightnessService = new SystemBrightnessService();

        await brightnessService.SetBrightnessAsync(brightness);
    }

    [Description("Increases screen brightness")]
    public static async Task IncreaseBrightness(
        [Description("amount to increase brightness by")]
        int amount = 5)
    {
        var brightnessService = new SystemBrightnessService();

        await brightnessService.IncreaseAsync(amount);
    }

    [Description("Decreases screen brightness")]
    public static async Task DecreaseBrightness(
        [Description("amount to decrease brightness by")]
        int amount = 5)
    {
        var brightnessService = new SystemBrightnessService();

        await brightnessService.DecreaseAsync(amount);
    }
}