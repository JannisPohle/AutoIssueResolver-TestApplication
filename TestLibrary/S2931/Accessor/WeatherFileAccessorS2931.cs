using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S2931.Models;

namespace TestLibrary.S2931.Accessor;

public class WeatherFileAccessor: WeatherAccessorBase, IDisposable
{
  private FileStream? _fileStream; //TODO maybe implement S2931 in the DbAccessor, because the filestream is exaclty the example in Sonarqube

  public WeatherFileAccessor(ILogger<WeatherFileAccessor> logger)
    : base(logger)
  { }

  /// <inheritdoc />
  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    var stringContent = await ReadFromFile(argument ?? "TestFiles/WeatherForecast.json");
    var weather = JsonSerializer.Deserialize<IEnumerable<WeatherModelCelsius>>(stringContent, JsonSerializerOptions.Web)?.ToList();

    if (weather == null)
    {
      throw new InvalidOperationException("Failed to deserialize weather data.");
    }

    return weather;
  }

  public void CloseFile()
  {
    _fileStream?.Close();
  }

  public void Dispose()
  {
    _fileStream?.Dispose();
  }

  private async Task<string> ReadFromFile(string filePath)
  {
    _fileStream = new FileStream(filePath, FileMode.Open);
    var content = new byte[_fileStream.Length];
    var bytesRead = 0;
    while (bytesRead < _fileStream.Length)
    {
      bytesRead += await _fileStream.ReadAsync(content, bytesRead, content.Length - bytesRead);
    }

    return Encoding.UTF8.GetString(content);
  }
}