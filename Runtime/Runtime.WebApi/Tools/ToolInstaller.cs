using Microsoft.Extensions.AI;

namespace Runtime.WebApi.Tools;

public static class ToolInstaller
{
    public static ChatOptions InstallTools(this ChatOptions options)
    {
        options.Tools =
        [
            AIFunctionFactory.Create(SystemBrightnessTools.DecreaseBrightness),
            AIFunctionFactory.Create(SystemBrightnessTools.IncreaseBrightness),
            AIFunctionFactory.Create(SystemBrightnessTools.SetBrightness),
            AIFunctionFactory.Create(SystemBrightnessTools.GetCurrentScreenBrightness),

            AIFunctionFactory.Create(WeatherTool.GetWeather),

            AIFunctionFactory.Create(SystemSoundTools.SetVolume),
            AIFunctionFactory.Create(SystemSoundTools.SetMute),
            AIFunctionFactory.Create(SystemSoundTools.GetCurrentSystemVolume)
        ];

        return options;
    }
}