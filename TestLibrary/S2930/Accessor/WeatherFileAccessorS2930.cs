using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S2930.Models;

namespace TestLibrary.S2930.Accessor;

public class WeatherFileAccessor: WeatherAccessorBase, IDisposable
{
  public WeatherFileAccessor(ILogger<WeatherFileAccessor> logger)
    : base(logger)
  { }

  /// <inheritdoc />
  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    var stringContent = await ReadFromFile(argument ?? "TestFiles/WeatherForecast.json");
    var weather = JsonSerializer.Deserialize<IEnumerable<WeatherModelCelsius>>(stringContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })?.ToList();

    if (weather == null)
    {
      throw new InvalidOperationException("Failed to deserialize weather data.");
    }

    return weather;
  }

  private static async Task<string> ReadFromFile(string filePath)
  {
    using FileStream fs = File.Open(filePath, FileMode.Open);
    var content = new byte[fs.Length];
    await fs.ReadAsync(content, 0, content.Length);
    return Encoding.UTF8.GetString(content);
  }

  public void Dispose()
  {
    // Implement any cleanup logic if necessary
  }
}