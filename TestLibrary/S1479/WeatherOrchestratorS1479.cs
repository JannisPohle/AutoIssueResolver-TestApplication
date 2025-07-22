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

      _logger.LogInformation("Retrieved {Count} weather records", result.Count);

      return Result<List<WeatherModelCelsius>>.Success(result);
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error retrieving weather data");

      return Result<List<WeatherModelCelsius>>.Failure(e);
    }
  }
}
