using System.Text.Json; 
using Microsoft.Extensions.Logging;
using TestLibrary.S110.Models;

namespace TestLibrary.S110.Accessor;

public class WeatherFileAccessor
{
  private readonly ILogger<WeatherFileAccessor> _logger;

  public WeatherFileAccessor(ILogger<WeatherFileAccessor> logger)
  {
    _logger = logger;
  }

  public async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    var filePath = argument ?? "TestFiles/WeatherForecast.json";
    if (!File.Exists(filePath))
    {
      _logger.LogWarning("Weather data file not found: {FilePath}", filePath);
      throw new FileNotFoundException($"Weather data file not found: {filePath}");
    }

    var stringContent = await ReadFromFile(argument ?? "TestFiles/WeatherForecast.json");
    var weather = JsonSerializer.Deserialize<IEnumerable<WeatherModelCelsius>>(stringContent, JsonSerializerOptions.Web)?.ToList();

    if (weather == null)
    {
      _logger.LogWarning("Failed to deserialize weather data from file: {FilePath}", argument);
      throw new InvalidOperationException("Failed to deserialize weather data.");
    }

    return weather;
  }

  private static async Task<string> ReadFromFile(string filePath)
  {
    await using var fs = new FileStream(filePath, FileMode.Open);
    var content = new byte[fs.Length];
    var bytesRead = 0;
    while (bytesRead < fs.Length)
    {
      bytesRead += await fs.ReadAsync(content, bytesRead, content.Length - bytesRead);
    }

    return Encoding.UTF8.GetString(content);
  }
}