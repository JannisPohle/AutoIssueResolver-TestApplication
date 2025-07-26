using Microsoft.Extensions.Logging;
using TestLibrary.S3358.Abstractions;
using TestLibrary.S3358.Accessor;
using TestLibrary.S3358.Models;

namespace TestLibrary.S3358;

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
      var validationResult = mode == AccessMode.None ? Result<List<WeatherModelCelsius>>.Failure(new ArgumentException("Access mode must be specified", nameof(mode)))
                         : !Enum.IsDefined(typeof(AccessMode), mode) ? Result<List<WeatherModelCelsius>>.Failure( new ArgumentOutOfRangeException(nameof(mode), mode, null)) : null;
      if (validationResult != null)
      {
        return validationResult;
      }

      _logger.LogInformation("Getting weather from {AccessMode} with Argument: {Argument}", mode, argument);

      List<WeatherModelCelsius> result;
      switch (mode)
      {
        case AccessMode.File:
          result = await _fileAccessor.GetWeather(argument);
          break;
        case AccessMode.Mock:
          result = await _mockAccessor.GetWeather(argument);
          break;
        case AccessMode.Database:
          result = await GetWeatherDataFromDbAccessor(argument);
          break;
        case AccessMode.Web:
          result = await _apiAccessor.GetWeather(argument);
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

  private async Task<List<WeatherModelCelsius>> GetWeatherDataFromDbAccessor(string? argument)
  {
    await _dbAccessor.OpenConnection(argument);
    return await _dbAccessor.GetWeather(argument);
  }
}