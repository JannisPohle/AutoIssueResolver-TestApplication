using Microsoft.AspNetCore.Mvc;
using TestLibrary.S6962.Models;

namespace WebApplication.Controllers.S6962;

[ApiController]
[Route("[controller]")]
public class S9662Controller: ControllerBase, IDisposable
{
  protected virtual void Dispose(bool disposing)
  {
    if (disposing)
    {
      _httpClient.Dispose();
    }
  }

  [ApiExplorerSettings(IgnoreApi = true)]
  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  private readonly HttpClient _httpClient;
  private readonly ILogger<S9662Controller> _logger;

  public S9662Controller(ILogger<S9662Controller> logger)
  {
    _logger = logger;
    _httpClient = new HttpClient();
  }

  [HttpGet("external")]
  public async Task<IActionResult> GetWeatherForecast([FromQuery] string? argument = null, [FromQuery] bool throwOnError = false)
  {
    try
    {
      _logger.LogTrace("Get WeatherForecast");

      var result = await _httpClient.GetFromJsonAsync<List<WeatherModelCelsius>>($"http://localhost:31246/api/weather?location={argument}");

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