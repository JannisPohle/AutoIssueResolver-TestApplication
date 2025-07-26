using Microsoft.Extensions.Logging;
using TestLibrary.S2696.Models;

namespace TestLibrary.S2696.Accessor;

public class WeatherMockAccessor: WeatherAccessorBase
{
  private int _callCount = 0;

  public WeatherMockAccessor(ILogger<WeatherMockAccessor> logger)
    : base(logger)
  { }

  public override Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    _callCount++;
    var weather = GenerateWeatherData(argument).ToList();

    if (!weather.Any())
    {
      throw new DataNotFoundException("No weather data available.");
    }

    return Task.FromResult(weather.ToList());
  }

  private IEnumerable<WeatherModelCelsius> GenerateWeatherData(string? argument)
  {
    var random = string.IsNullOrWhiteSpace(argument) ? new Random(_callCount) : new Random(argument?.GetHashCode() ?? 0);

    int count;
    if (string.IsNullOrWhiteSpace(argument))
    {
      count = _callCount; // Default to the number of calls to the accessor if no argument is provided
    }
    else if (!int.TryParse(argument, out count))
    {
      count = 10; // If an argument is provided, try to parse it as an integer. If not possible, fallback to 10
    }


    for (var i = 0; i < count; i++)
    {
      yield return new WeatherModelCelsius(random.Next(-20, 45));
    }
  }
}