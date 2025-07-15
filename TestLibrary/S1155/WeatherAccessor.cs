using System.Text.Json;

namespace TestLibrary.S1155;

public class WeatherAccessor
{
  private readonly string _filePath;

  public WeatherAccessor(string filePath)
  {
    _filePath = filePath;
  }

  public virtual async Task<IEnumerable<WeatherForecast>> LoadWeatherForecasts()
  {
    await using var fileStream = new FileStream(_filePath, FileMode.Open);
    using var reader = new StreamReader(fileStream);
    var forecasts = (await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(fileStream)) ?? [];

    if (forecasts.Count() == 0)
    {
      throw new InvalidOperationException("The file is empty.");
    }

    return forecasts;
  }
}