using Microsoft.Extensions.Logging;
using TestLibrary.S4019.Models;

namespace TestLibrary.S4019.Accessor;

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

  public override bool ValidateWeatherData(WeatherModelCelsius data)
  {
    var success = !string.IsNullOrEmpty(data.Unit);
    Logger.LogTrace("Validate weather data : {Success}", success);
    return success;
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
      yield return new WeatherModelCelsius(random.Next(-80, -45));
    }
  }
}
