using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using FileAccess = TestLibrary.FileAccess;

namespace WebApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController: ControllerBase
{
  private readonly ILogger<WeatherForecastController> _logger;
  private readonly FileAccess _fileAccess;

  public WeatherForecastController(ILogger<WeatherForecastController> logger, FileAccess fileAccess)
  {
    _logger = logger;
    _fileAccess = fileAccess;
  }

  [HttpGet(Name = "GetWeatherForecast")]
  public IEnumerable<WeatherForecast> Get()
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
}