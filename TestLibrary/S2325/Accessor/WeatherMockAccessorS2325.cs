using Microsoft.Extensions.Logging;
using TestLibrary.S2325.Models;

namespace TestLibrary.S2325.Accessor;

public class WeatherMockAccessor: WeatherAccessorBase
{
  public WeatherMockAccessor(ILogger<WeatherMockAccessor> logger)
    : base(logger)
  { }

  public override Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    var weather = GenerateWeatherData(argument);

    if (!weather.Any())
    {
      throw new DataNotFoundException("No weather data available.");
    }

    return Task.FromResult(weather.ToList());
  }

  private static IEnumerable<WeatherModelCelsius> GenerateWeatherData(string? argument)
  {
    var random = string.IsNullOrWhiteSpace(argument) ? new Random() : new Random(argument?.GetHashCode() ?? 0);

    if (!int.TryParse(argument, out var count))
    {
      count = 10; // Default count if parsing fails
    }

    for (var i = 0; i < count; i++)
    {
      yield return new WeatherModelCelsius(random.Next(-20, 45));
    }
  }
}
