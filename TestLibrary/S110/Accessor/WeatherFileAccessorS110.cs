`
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S110.Models;

namespace TestLibrary.S110.Accessor;

public class WeatherFileAccessor(ILogger<WeatherFileAccessor> logger): LoggerBase(logger)
{
  public Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    var filePath = argument ?? "TestFiles/WeatherForecast.json";
    if (!File.Exists(filePath))
    {
      Logger.LogWarning("Weather data file not found: {FilePath}", filePath);
      throw new FileNotFoundException($"Weather data file not found: {filePath}");
    }

    var stringContent = ReadFromFile(argument ?? "TestFiles/WeatherForecast.json");
    var weather = JsonSerializer.Deserialize<IEnumerable<WeatherModelCelsius>>(stringContent, JsonSerializerOptions.Web)?.ToList();

    if (weather == null)
    {
      Logger.LogWarning("Failed to deserialize weather data from file: {FilePath}", argument);
      throw new InvalidOperationException("Failed to deserialize weather data.");
    }

    return Task.FromResult(weather);
  }

  private static string ReadFromFile(string filePath)
  {
    using var fs = new FileStream(filePath, FileMode.Open);
    using var reader = new StreamReader(fs);
    return reader.ReadToEnd();
  }
}
`