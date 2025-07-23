using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using TestLibrary.S3776.Abstractions;
using TestLibrary.S3776.Accessor;
using TestLibrary.S3776.Models;

namespace TestLibrary.S3776;

public class WeatherOrchestrator: IWeatherOrchestrator
{
  #region Members

  private readonly WeatherApiAccessor _apiAccessor;
  private readonly WeatherDbAccessor _dbAccessor;
  private readonly WeatherFileAccessor _fileAccessor;
  private readonly ILogger<WeatherOrchestrator> _logger;
  private readonly WeatherMockAccessor _mockAccessor;

  #endregion

  #region Constructors

  public WeatherOrchestrator(WeatherFileAccessor fileAccessor, WeatherDbAccessor dbAccessor, WeatherApiAccessor apiAccessor, WeatherMockAccessor mockAccessor, ILogger<WeatherOrchestrator> logger)
  {
    _fileAccessor = fileAccessor;
    _dbAccessor = dbAccessor;
    _apiAccessor = apiAccessor;
    _mockAccessor = mockAccessor;
    _logger = logger;
  }

  #endregion

  #region Methods

  public async Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessMode mode, string? argument = null)
  {
    Result<List<WeatherModelCelsius>> finalResult;

    try
    {
      if (mode != AccessMode.None)
      {
        if (Enum.IsDefined(typeof(AccessMode), mode))
        {
          _logger.LogInformation("Getting weather from {AccessMode} with Argument: {Argument}", mode, argument);

          var result = new List<WeatherModelCelsius>();

          if (mode == AccessMode.Database)
          {
            await _dbAccessor.OpenConnection(argument);
            result.AddRange(await _dbAccessor.GetWeather(argument));
          }
          else if (mode == AccessMode.File)
          {
            result.AddRange(await _fileAccessor.GetWeather(argument));
          }
          else if (mode == AccessMode.Web)
          {
            result.AddRange(await _apiAccessor.GetWeather(argument));
          }
          else if (mode == AccessMode.Mock)
          {
            result.AddRange(await _mockAccessor.GetWeather(argument));
          }
          else
          {
            throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
          }

          foreach (var weather in result)
          {
            if (weather.Temperature > 120)
            {
              throw new ValidationException("Temperature cannot exceed 120 degrees Celsius");
            }

            if (weather.Temperature < -100)
            {
              throw new ValidationException("Temperature cannot be below -100 degrees Celsius");
            }

            if (string.IsNullOrWhiteSpace(weather.Unit))
            {
              throw new ValidationException("Unit cannot be null or empty");
            }
          }

          _logger.LogInformation("Retrieved {Count} weather records", result.Count);

          finalResult = Result<List<WeatherModelCelsius>>.Success(result);
        }
        else
        {
          finalResult = Result<List<WeatherModelCelsius>>.Failure(new ArgumentOutOfRangeException(nameof(mode), mode, null));
        }
      }
      else
      {
        finalResult = Result<List<WeatherModelCelsius>>.Failure(new ArgumentException("Access mode must be specified", nameof(mode)));
      }
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error retrieving weather data");

      finalResult = Result<List<WeatherModelCelsius>>.Failure(e);
    }

    return finalResult;
  }

  #endregion
}