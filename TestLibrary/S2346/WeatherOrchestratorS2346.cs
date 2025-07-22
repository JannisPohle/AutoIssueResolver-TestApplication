using Microsoft.Extensions.Logging;
using TestLibrary.S2346.Abstractions;
using TestLibrary.S2346.Accessor;
using TestLibrary.S2346.Models;

namespace TestLibrary.S2346;

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


  public async Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessModes mode, string? argument = null)
  {
    try
    {
      if (mode == AccessModes.Nothing)
      {
        return Result<List<WeatherModelCelsius>>.Failure(new ArgumentException("Access mode must be specified", nameof(mode)));
      }

      _logger.LogInformation("Getting weather from {AccessMode} with Argument: {Argument}", mode, argument);

      var result = new List<WeatherModelCelsius>();
      if (mode.HasFlag(AccessModes.File))
      {
        try
        {
          result.AddRange(await _fileAccessor.GetWeather(argument));
        }
        catch (Exception e) when (mode != AccessModes.File) // if other data sources are available, we want to log the error but not fail
        {
          _logger.LogWarning(e, "Failed to retrieve weather data from file. Continuing with other sources.");
        }
      }

      if (mode.HasFlag(AccessModes.Mock))
      {
        try
        {
          result.AddRange(await _mockAccessor.GetWeather(argument));
        }
        catch (Exception e) when (mode != AccessModes.Mock) // if other data sources are available, we want to log the error but not fail
        {
          _logger.LogWarning(e, "Failed to retrieve weather data from mock accessor. Continuing with other sources.");
        }
      }

      if (mode.HasFlag(AccessModes.Database))
      {
        try
        {
          result.AddRange(await GetWeatherDataFromDbAccessor(argument));
        }
        catch (Exception e) when (mode != AccessModes.Database)  // if other data sources are available, we want to log the error but not fail
        {
          _logger.LogWarning(e, "Failed to retrieve weather data from database. Continuing with other sources.");
        }
      }

      if (mode.HasFlag(AccessModes.Web))
      {
        try
        {
          result.AddRange(await _apiAccessor.GetWeather(argument));
        }
        catch (Exception e) when (mode != AccessModes.Web) // if other data sources are available, we want to log the error but not fail
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

  private async Task<List<WeatherModelCelsius>> GetWeatherDataFromDbAccessor(string? argument)
  {
    await _dbAccessor.OpenConnection(argument);
    return await _dbAccessor.GetWeather(argument);
  }
}
