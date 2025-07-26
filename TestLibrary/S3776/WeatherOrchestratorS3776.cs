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
    try
    {
      if (mode == AccessMode.None)
      {
        return Result<List<WeatherModelCelsius>>.Failure(new ArgumentException("Access mode must be specified", nameof(mode)));
      }

      if (!Enum.IsDefined(typeof(AccessMode), mode))
      {
        return Result<List<WeatherModelCelsius>>.Failure(new ArgumentOutOfRangeException(nameof(mode), mode, null));
      }

      _logger.LogInformation("Getting weather from {AccessMode} with Argument: {Argument}", mode, argument);

      var result = await RetrieveWeatherAsync(mode, argument);

      ValidateWeatherList(result);

      _logger.LogInformation("Retrieved {Count} weather records", result.Count);

      return Result<List<WeatherModelCelsius>>.Success(result);
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error retrieving weather data");
      return Result<List<WeatherModelCelsius>>.Failure(e);
    }
  }

  private async Task<List<WeatherModelCelsius>> RetrieveWeatherAsync(AccessMode mode, string? argument)
  {
    return mode switch
    {
      AccessMode.Database => await GetFromDatabase(argument),
      AccessMode.File => await _fileAccessor.GetWeather(argument),
      AccessMode.Web => await _apiAccessor.GetWeather(argument),
      AccessMode.Mock => await _mockAccessor.GetWeather(argument),
      _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
    };
  }

  private async Task<List<WeatherModelCelsius>> GetFromDatabase(string? argument)
  {
    await _dbAccessor.OpenConnection(argument);
    return await _dbAccessor.GetWeather(argument);
  }

  private static void ValidateWeatherList(IEnumerable<WeatherModelCelsius> weatherList)
  {
    foreach (var weather in weatherList)
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
  }

  #endregion
}
