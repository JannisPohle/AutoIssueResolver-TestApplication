using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S4015.Models;

namespace TestLibrary.S4015.Accessor;

public class WeatherFileAccessor: WeatherAccessorBase
{
  public WeatherFileAccessor(ILogger<WeatherFileAccessor> logger)
    : base(logger)
  { }

  /// <inheritdoc />
  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    ValidateArgument(argument);

    var stringContent = await ReadFromFile(argument ?? "TestFiles/WeatherForecast.json");
    var weather = JsonSerializer.Deserialize<IEnumerable<WeatherModelCelsius>>(stringContent, JsonSerializerOptions.Web)?.ToList();

    if (weather == null)
    {
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

  protected static void ValidateArgument(string? argument)
  {
    if (argument == null)
    {
      return;
    }

    if (argument.Trim().Length == 0)
    {
      throw new ArgumentException("Argument cannot be an empty string.", nameof(argument));
    }

    if (argument.Length > 200)
    {
      throw new ArgumentException("Argument cannot exceed 100 characters.", nameof(argument));
    }
  }
}
