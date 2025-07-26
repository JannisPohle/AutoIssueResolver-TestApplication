using Microsoft.AspNetCore.Mvc;
using TestLibrary.S3427;
using TestLibrary.S3427.Models;

namespace WebApplication.Controllers.S3427;

[ApiController]
[Route("[controller]")]
public class S3427Controller: ControllerBase
{
  #region Members

  private readonly ILogger<S3427Controller> _logger;
  private readonly WeatherOrchestrator _weatherOrchestrator;

  #endregion

  #region Constructors

  public S3427Controller(ILogger<S3427Controller> logger, WeatherOrchestrator weatherOrchestrator)
  {
    _logger = logger;
    _weatherOrchestrator = weatherOrchestrator;
  }

  #endregion

  #region Methods

  [HttpGet]
  public async Task<IActionResult> GetWeatherForecast([FromQuery] string? argument = null, [FromQuery] bool throwOnError = false)
  {
    try
    {
      _logger.LogTrace("Get WeatherForecast");

      var result = await _weatherOrchestrator.GetWeatherWithThrow(AccessMode.File, argument, throwOnError);

      if (!result.IsSuccess)
      {
        _logger.LogError(result.Exception, "Failed to retrieve weather data");

        return Ok(new List<WeatherModelCelsius>());
      }

      return Ok(result.Payload!);
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error reading WeatherForecast data");

      return BadRequest();
    }
  }

  #endregion
}
