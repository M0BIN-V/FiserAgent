using System.ComponentModel;
using Runtime.Application.Common.Abstractions;

namespace Runtime.Application.Tools;

public class WeatherTool : ITool
{
    [Description("Get the weather for a given location.")]
    public string GetWeather([Description("The location to get the weather for.")] string location)
    {
        return $"The weather in {location} is cloudy with a high of 15°C.";
    }
}