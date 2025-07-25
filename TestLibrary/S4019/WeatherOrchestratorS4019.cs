using Microsoft.Extensions.Logging;
using TestLibrary.S4019.Abstractions;
using TestLibrary.S4019.Accessor;
using TestLibrary.S4019.Models;

namespace TestLibrary.S4019;

public class WeatherOrchestrator: IWeatherOrchestrator
{
  private const string VALIDATION_MESSAGE = "Invalid weather data encountered";

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
        AccessMode.File => await GetFromFile(argument),
        AccessMode.Mock => await GetFromMock(argument),
        AccessMode.Database => await GetFromDb(argument),
        AccessMode.Web => await GetFromApi(argument),
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

  private async Task<List<WeatherModelCelsius>> GetFromApi(string? argument)
  {
    var data = await _apiAccessor.GetWeather(argument);
    foreach (var weatherData in data)
    {
      var success = _apiAccessor.ValidateWeatherData(weatherData);

      if (!success)
      {
        throw new ArgumentException(VALIDATION_MESSAGE);
      }
    }
    return data;
  }

  private async Task<List<WeatherModelCelsius>> GetFromMock(string? argument)
  {
    var data = await _mockAccessor.GetWeather(argument);
    foreach (var weatherData in data)
    {
      var success = _mockAccessor.ValidateMockWeatherData(weatherData);

      if (!success)
      {
        throw new ArgumentException(VALIDATION_MESSAGE);
      }
    }
    return data;
  }

  private async Task<List<WeatherModelCelsius>> GetFromFile(string? argument)
  {
    var data = await _fileAccessor.GetWeather(argument);
    foreach (var weatherData in data)
    {
      var success = _fileAccessor.ValidateWeatherData(weatherData);

      if (!success)
      {
        throw new ArgumentException(VALIDATION_MESSAGE);
      }
    }
    return data;
  }

  private async Task<List<WeatherModelCelsius>> GetFromDb(string? argument)
  {
    await _dbAccessor.OpenConnection(argument);
    var data = await _dbAccessor.GetWeather(argument);
    foreach (var weatherData in data)
    {
      var success = _dbAccessor.ValidateWeatherData(weatherData);

      if (!success)
      {
        throw new ArgumentException(VALIDATION_MESSAGE);
      }
    }

    return data;
  }
}
