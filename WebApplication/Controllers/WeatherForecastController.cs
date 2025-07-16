using Microsoft.AspNetCore.Mvc;
using TestLibrary.Template.Abstractions;
using TestLibrary.Template.Models;

namespace WebApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController: ControllerBase
{
  private readonly ILogger<WeatherForecastController> _logger;
  private readonly IWeatherOrchestrator _weatherAccess;

  public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherOrchestrator weatherAccess)
  {
    _logger = logger;
    _weatherAccess = weatherAccess;
  }

  [HttpGet]
  [Route("Template")]
  public async Task<IEnumerable<WeatherModelCelsius>> GetWeatherForecast()
  {
    try
    {
      _logger.LogTrace("Get WeatherForecast");

      var result = await _weatherAccess.GetWeather(AccessMode.File);
      if (!result.IsSuccess)
      {
        _logger.LogError(result.Exception, "Failed to retrieve weather data");
        return [];
      }

      return result.Payload!;
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error reading WeatherForecast data");
      return [];
    }
  }
}