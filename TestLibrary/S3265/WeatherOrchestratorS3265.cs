using Microsoft.Extensions.Logging;
using TestLibrary.S3265.Abstractions;
using TestLibrary.S3265.Accessor;
using TestLibrary.S3265.Models;

namespace TestLibrary.S3265;

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
      if (((AccessMode.File | AccessMode.Database | AccessMode.Web | AccessMode.Mock) & mode ) == 0)
      {
        return Result<List<WeatherModelCelsius>>.Failure(new ArgumentException("Access mode must be specified", nameof(mode)));
      }

      _logger.LogInformation("Getting weather from {AccessMode} with Argument: {Argument}", mode, argument);

      var result = new List<WeatherModelCelsius>();
      if (mode.HasFlag(AccessMode.File))
      {
        try
        {
          result.AddRange(await _fileAccessor.GetWeather(argument));
        }
        catch (Exception e) when (mode != AccessMode.File) // if other data sources are available, we want to log the error but not fail
        {
          _logger.LogWarning(e, "Failed to retrieve weather data from file. Continuing with other sources.");
        }
      }

      if (mode.HasFlag(AccessMode.Mock))
      {
        try
        {
          result.AddRange(await _mockAccessor.GetWeather(argument));
        }
        catch (Exception e) when (mode != AccessMode.Mock) // if other data sources are available, we want to log the error but not fail
        {
          _logger.LogWarning(e, "Failed to retrieve weather data from mock accessor. Continuing with other sources.");
        }
      }

      if (mode.HasFlag(AccessMode.Database))
      {
        try
        {
          result.AddRange(await _dbAccessor.GetWeather(argument));
        }
        catch (Exception e) when (mode != AccessMode.Database)  // if other data sources are available, we want to log the error but not fail
        {
          _logger.LogWarning(e, "Failed to retrieve weather data from database. Continuing with other sources.");
        }
      }

      if (mode.HasFlag(AccessMode.Web))
      {
        try
        {
          result.AddRange(await _apiAccessor.GetWeather(argument));
        }
        catch (Exception e) when (mode != AccessMode.Web) // if other data sources are available, we want to log the error but not fail
        {
          _logger.LogWarning(e, "Failed to retrieve weather data from web API. Continuing with other sources.");
        }
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
