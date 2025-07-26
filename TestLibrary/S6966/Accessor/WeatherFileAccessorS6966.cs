public class WeatherFileAccessor: WeatherAccessorBase
{
  public WeatherFileAccessor(ILogger<WeatherFileAccessor> logger)
    : base(logger)
  { }

  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    var filePath = argument ?? "defaultFilePath.json";
    
    try
    {
      var json = await ReadFromFile(filePath);
      var weatherData = JsonSerializer.Deserialize<IEnumerable<WeatherModelCelsius>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?.ToList();

      if (weatherData == null)
      {
        throw new DataNotFoundException("Failed to deserialize weather data.");
      }

      return await Task.FromResult(weatherData);
    }
    catch (Exception e)
    {
      throw new InvalidOperationException($"Error reading from file: {filePath}", e);
    }
  }

  private static async Task<string> ReadFromFile(string filePath)
  {
    using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
    using var sr = new StreamReader(fs);
    
    return await sr.ReadToEndAsync();
  }
}