using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TestLibrary;
using TestLibrary.S3442;
using FileAccess = TestLibrary.S2930.FileAccess;

namespace WebApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController: ControllerBase
{
  private readonly ILogger<WeatherForecastController> _logger;
  private readonly FileAccess _fileAccess;
  private readonly TestLibrary.S2931.FileAccess _fileAccessS2931;

  public WeatherForecastController(ILogger<WeatherForecastController> logger, FileAccess fileAccess, TestLibrary.S2931.FileAccess fileAccessS2931)
  {
    _logger = logger;
    _fileAccess = fileAccess;
    _fileAccessS2931 = fileAccessS2931;
  }

  [HttpGet]
  [Route("S2930")]
  public IEnumerable<WeatherForecast> GetWeatherForecastS2930()
  {
    try
    {
      _logger.LogTrace("Get WeatherForecast");
      var content = _fileAccess.ReadFromFile("TestFiles/WeatherForecast.json");
      return JsonSerializer.Deserialize<List<WeatherForecast>>(content) ?? [];
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error reading WeatherForecast data");
      return [];
    }
  }

  [HttpGet]
  [Route("S2931")]
  public IEnumerable<WeatherForecast> GetWeatherForecastS2931()
  {
    try
    {
      _logger.LogTrace("Get WeatherForecast S2931");
      _fileAccessS2931.OpenWeatherForecastFile();
      var content = _fileAccessS2931.ReadWeatherForecastFile();
      _fileAccessS2931.CloseWeatherForecastFile();
      return JsonSerializer.Deserialize<List<WeatherForecast>>(content) ?? [];
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error reading WeatherForecast data");
      return [];
    }
  }

  [HttpGet]
  [Route("S3442")]
  public IEnumerable<WeatherCelsius> GetWeatherForecastS3442()
  {
    try
    {
      _logger.LogTrace("Get WeatherForecast S2931");
      var weather = new WeatherCelsius(Random.Shared.Next(-20, 50));

      return new List<WeatherCelsius> { weather, };
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error reading WeatherForecast data");
      return [];
    }
  }
}