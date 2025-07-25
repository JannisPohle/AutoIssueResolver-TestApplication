using Microsoft.Extensions.Logging;
using TestLibrary.S1479.Abstractions;
using TestLibrary.S1479.Accessor;
using TestLibrary.S1479.Models;

namespace TestLibrary.S1479;

public class WeatherOrchestrator: IWeatherOrchestrator
{
  private readonly WeatherApiAccessor _apiAccessor;
  private readonly WeatherDbAccessor _dbAccessor;
  private readonly WeatherMockAccessor _mockAccessor;
  private readonly WeatherFileAccessor _fileAccessor;
  private readonly ILogger<WeatherOrchestrator> _logger;

  public WeatherOrchestrator(WeatherFileAccessor fileAccessor, WeatherDbAccessor dbAccessor, WeatherApiAccessor apiAccessor, WeatherMockAccessor mockAccessor, ILogger<WeatherOrchestrator> logger)
  {
    _fileAccessor = fileAccessor;
    _dbAccessor = dbAccessor;
    _apiAccessor = apiAccessor;
    _mockAccessor = mockAccessor;
    _logger = logger;
  }


  public async Task<Result<List<WeatherModelCelsius>>> GetWeather(string mode, string? argument = null)
  {
    try
    {
      if (mode == "None")
      {
        return Result<List<WeatherModelCelsius>>.Failure(new ArgumentException("Access mode must be specified", nameof(mode)));
      }

      _logger.LogInformation("Getting weather from {AccessMode} with Argument: {Argument}", mode, argument);


      var result = new List<WeatherModelCelsius>();
      switch (mode)
      {
        case "File":
          result.AddRange(await GetWeatherFromFile(argument));
          break;
        case "Mock":
          result.AddRange(await GetWeatherFromMock(argument));
          break;
        case "Database":
          result.AddRange(await GetWeatherFromDatabase(argument));
          break;
        case "Web":
          result.AddRange(await GetWeatherFromWeb(argument));
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
      }

      _logger.LogInformation("Retrieved {Count} weather records", result.Count);

      return Result<List<WeatherModelCelsius>>.Success(result);
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error retrieving weather data");

      return Result<List<WeatherModelCelsius>>.Failure(e);
    }
  }

  private async Task<List<WeatherModelCelsius>> GetWeatherFromFile(string? argument)
  {
    return await _fileAccessor.GetWeather(argument);
  }

  private async Task<List<WeatherModelCelsius>> GetWeatherFromMock(string? argument)
  {
    return await _mockAccessor.GetWeather(argument);
  }

  private async Task<List<WeatherModelCelsius>> GetWeatherFromDatabase(string? argument)
  {
    await _dbAccessor.OpenConnection(argument);
    return await _dbAccessor.GetWeather(argument);
  }

  private async Task<List<WeatherModelCelsius>> GetWeatherFromWeb(string? argument)
  {
    return await _apiAccessor.GetWeather(argument);
  }
}