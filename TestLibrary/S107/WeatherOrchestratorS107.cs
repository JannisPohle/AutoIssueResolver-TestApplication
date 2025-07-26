using Microsoft.Extensions.Logging;
using TestLibrary.S107.Abstractions;
using TestLibrary.S107.Accessor;
using TestLibrary.S107.Models;

namespace TestLibrary.S107;

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
        AccessMode.Web => await GetWeatherFromApi(argument),
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

  private async Task<List<WeatherModelCelsius>> GetWeatherFromApi(string? argument)
  {
    var arguments = argument?.Split(';') ?? [];
    var request = new WeatherApiRequest(
      arguments.ElementAtOrDefault(0),
      arguments.ElementAtOrDefault(1),
      arguments.ElementAtOrDefault(2),
      arguments.ElementAtOrDefault(3),
      arguments.ElementAtOrDefault(4),
      arguments.ElementAtOrDefault(5)
    );
    return await _apiAccessor.GetWeather(request);
  }

  private async Task<List<WeatherModelCelsius>> GetWeatherDataFromDbAccessor(string? argument)
  {
    await _dbAccessor.OpenConnection(argument);
    return await _dbAccessor.GetWeather(argument);
  }
}
