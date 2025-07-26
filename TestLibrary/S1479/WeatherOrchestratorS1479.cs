// New private method to encapsulate switch case logic
private async Task<List<WeatherModelCelsius>> GetWeatherData(string mode, string? argument)
{
  var result = new List<WeatherModelCelsius>();
  switch (mode)
  {
    case "File":
      result.AddRange(await _fileAccessor.GetWeather(argument));
      break;
    case "Mock":
      result.AddRange(await _mockAccessor.GetWeather(argument));
      break;
    case "Database":
      await _dbAccessor.OpenConnection(argument);
      result.AddRange(await _dbAccessor.GetWeather(argument));
      break;
    case "Web":
      result.AddRange(await _apiAccessor.GetWeather(argument));
      break;
    default:
      throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
  }
  return result;
}