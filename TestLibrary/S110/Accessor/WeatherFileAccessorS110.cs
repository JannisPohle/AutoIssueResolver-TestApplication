using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S110.Models;

namespace TestLibrary.S110.Accessor;

public class WeatherFileAccessor: WeatherAccessorBase
{
  public WeatherFileAccessor(ILogger<WeatherFileAccessor> logger)
    : base(logger)
  { }

  /// <inheritdoc />
  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    var filePath = argument ?? "TestFiles/WeatherForecast.json";
    if (!File.Exists(filePath))
    {
      Logger.LogWarning("Weather data file not found: {FilePath}", filePath);
      throw new FileNotFoundException($"Weather data file not found: {filePath}");
    }

    var stringContent = await ReadFromFile(argument ?? "TestFiles/WeatherForecast.json");
    var weather = JsonSerializer.Deserialize<IEnumerable<WeatherModelCelsius>>(stringContent, JsonSerializerOptions.Web)?.ToList();

    if (weather == null)
    {
      Logger.LogWarning("Failed to deserialize weather data from file: {FilePath}", argument);
      throw new InvalidOperationException("Failed to deserialize weather data.");
    }

    return weather;
  }

  private async Task<string> ReadFromFile(string filePath)
  {
    try
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
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error reading from file: {FilePath}", filePath);
      throw;
    }
  }
}
