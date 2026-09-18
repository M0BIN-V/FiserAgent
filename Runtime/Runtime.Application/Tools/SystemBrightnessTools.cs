using System.ComponentModel;
using Runtime.Application.Common.Abstractions;
using Runtime.Application.Services;

namespace Runtime.Application.Tools;

public class SystemBrightnessTools(SystemBrightnessService brightnessService) : ITool
{
    [Description("Returns current screen brightness percentage")]
    public Task<int> GetCurrentScreenBrightness()
    {
        return brightnessService.GetBrightnessAsync();
    }

    [Description("Sets screen brightness percentage")]
    public Task SetBrightness([Description("brightness percentage from 0 to 100")] int brightness)
    {
        return brightnessService.SetBrightnessAsync(brightness);
    }

    [Description("Increases screen brightness")]
    public Task IncreaseBrightness(
        [Description("amount to increase brightness by")]
        int amount = 5)
    {
        return brightnessService.IncreaseAsync(amount);
    }

    [Description("Decreases screen brightness")]
    public Task DecreaseBrightness([Description("amount to decrease brightness by")] int amount = 5)
    {
        return brightnessService.DecreaseAsync(amount);
    }
}