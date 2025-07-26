// File: Startup.cs
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
        // ...
    }
}

[ApiController]
[Route("[controller]")]
public class S6962Controller : ControllerBase
{
    private readonly ILogger<S6962Controller> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public S6962Controller(ILogger<S6962Controller> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    [HttpGet("external")]
    public async Task<IActionResult> GetWeatherForecast([FromQuery] string? argument = null)
    {
        try
        {
            _logger.LogTrace("Get WeatherForecast");

            var client = _clientFactory.CreateClient(); // Compliant (Basic usage)
            client.BaseAddress = new Uri("http://localhost:31246/api/");

            var result = await client.GetFromJsonAsync<List<WeatherModelCelsius>>("weather" + (string.IsNullOrWhiteSpace(argument) ? null : $"?location={argument}"));

            if (result == null || result.Count == 0)
            {
                _logger.LogWarning("No data found from API endpoint for location: {{Argument}}", argument);

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