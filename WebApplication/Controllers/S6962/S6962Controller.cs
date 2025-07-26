using Microsoft.Extensions.Logging; // Add this line

    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<S6962Controller> _logger;

    public S6962Controller(IHttpClientFactory clientFactory, ILogger<S6962Controller> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    [HttpGet("external")]
    public async Task<IActionResult> GetWeatherForecast([FromQuery] string? argument = null)
    {
        try
        {
            _logger.LogTrace("Get WeatherForecast");

            using (var client = _clientFactory.CreateClient())
            {
                var result = await client.GetFromJsonAsync<List<WeatherModelCelsius>>("weather" + (string.IsNullOrWhiteSpace(argument) ? null : $?{"location={argument}"}))
                ;

                if (result == null || result.Count == 0)
                {
                    _logger.LogWarning("No data found from API endpoint for location: {Argument}", argument);

                    return NotFound();
                }

                return Ok(result);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error reading WeatherForecast data");

            return BadRequest();
        }
    }