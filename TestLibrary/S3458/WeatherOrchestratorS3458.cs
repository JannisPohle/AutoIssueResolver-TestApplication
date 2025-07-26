using TestLibrary.S3458.Abstractions;
using TestLibrary.S3458.Accessor;
using TestLibrary.S3458.Models;

namespace TestLibrary.S3458;

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
      _logger.LogInformation("Getting weather from {AccessMode} with Argument: {Argument}", mode, argument);


      List<WeatherModelCelsius>? result = [];

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
          _logger.LogWarning("No valid access mode provided, returning empty list");
          break;
      }

      _logger.LogInformation("Retrieved {Count} weather records", result?.Count);

      return Result<List<WeatherModelCelsius>>.Success(result ?? []);
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
