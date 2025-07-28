using Microsoft.AspNetCore.Mvc;
using TestLibrary.S6962.Models;

namespace WebApplication.Controllers.S6962;

[ApiController]
[Route("[controller]")]
public class S6962Controller: ControllerBase
{
  private readonly ILogger<S6962Controller> _logger;

  public S6962Controller(ILogger<S6962Controller> logger)
  {
    _logger = logger;
  }

  [HttpGet("external")]
  public async Task<IActionResult> GetWeatherForecast([FromQuery] string? argument = null)
  {
    try
    {
      _logger.LogTrace("Get WeatherForecast");

      using var httpClient = new HttpClient
      {
        BaseAddress = new Uri("http://localhost:31246/api/"),
      };

      var result = await httpClient.GetFromJsonAsync<List<WeatherModelCelsius>>("weather" + (string.IsNullOrWhiteSpace(argument) ? null : $"?location={argument}"));

      if (result == null || result.Count == 0)
      {
        _logger.LogWarning("No data found from API endpoint for location: {Argument}", argument);

        return NotFound();
      }

      return Ok(result);
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error reading WeatherForecast data");

      return BadRequest();
    }
  }
}