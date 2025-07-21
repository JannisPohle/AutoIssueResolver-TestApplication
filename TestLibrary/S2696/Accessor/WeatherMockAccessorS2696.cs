using Microsoft.Extensions.Logging;
using TestLibrary.S2696.Models;

namespace TestLibrary.S2696.Accessor;

public class WeatherMockAccessor: WeatherAccessorBase
{
  private static int _callCount = 0;

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

  private static IEnumerable<WeatherModelCelsius> GenerateWeatherData(string? argument)
  {
    var random = string.IsNullOrWhiteSpace(argument) ? new Random(_callCount) : new Random(argument?.GetHashCode() ?? 0);

    if (!int.TryParse(argument, out var count))
    {
      count = _callCount; // Default count if parsing fails
    }

    for (var i = 0; i < count; i++)
    {
      yield return new WeatherModelCelsius(random.Next(-20, 45));
    }
  }
}
