using Microsoft.Extensions.Logging;
using TestLibrary.S6667.Abstractions;
using TestLibrary.S6667.Accessor;
using TestLibrary.S6667.Models;

namespace TestLibrary.S6667;

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


  public async Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessMode mode, string? argument = null)
  {
    try
    {
      if (mode == AccessMode.None)
      {
        return Result<List<WeatherModelCelsius>>.Failure(new ArgumentException("Access mode must be specified", nameof(mode)));
      }

      _logger.LogInformation("Getting weather from {AccessMode} with Argument: {Argument}", mode, argument);


      var result = mode switch
      {
        AccessMode.File => await _fileAccessor.GetWeather(argument),
        AccessMode.Mock => await _mockAccessor.GetWeather(argument),
        AccessMode.Database => await GetWeatherDataFromDbAccessor(argument),
        AccessMode.Web => await _apiAccessor.GetWeather(argument),
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
      };

      _logger.LogInformation("Retrieved {Count} weather records", result.Count);

      return Result<List<WeatherModelCelsius>>.Success(result);
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error retrieving weather data");

      return Result<List<WeatherModelCelsius>>.Failure(e);
    }
  }

  private async Task<List<WeatherModelCelsius>> GetWeatherDataFromDbAccessor(string? argument)
  {
    await _dbAccessor.OpenConnection(argument);
    return await _dbAccessor.GetWeather(argument);
  }
}