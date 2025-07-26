public class WeatherOrchestrator: IWeatherOrchestrator
{
  private readonly ILogger<WeatherOrchestrator> _logger;
  private readonly Dictionary<AccessMode, Func<string?, Task<List<WeatherModelCelsius>>>> _accessors;

  public WeatherOrchestrator(
    WeatherFileAccessor fileAccessor,
    WeatherDbAccessor dbAccessor,
    WeatherApiAccessor apiAccessor,
    WeatherMockAccessor mockAccessor,
    ILogger<WeatherOrchestrator> logger)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    _accessors = new Dictionary<AccessMode, Func<string?, Task<List<WeatherModelCelsius>>>>
    {
      { AccessMode.File, fileAccessor.GetWeather },
      { AccessMode.Mock, mockAccessor.GetWeather },
      { AccessMode.Database, async (arg) => await dbAccessor.OpenConnection(arg).ConfigureAwait(false); return await dbAccessor.GetWeather(arg).ConfigureAwait(false); },
      { AccessMode.Web, apiAccessor.GetWeather }
    };
  }

  public async Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessMode mode, string? argument = null)
  {
    _logger.LogInformation("Attempting to get weather data via {AccessMode} with Argument: {Argument}", mode, argument);
    
    if (mode == AccessMode.None)
    {
      return Result<List<WeatherModelCelsius>>.Failure(new ArgumentException("Access mode must be specified", nameof(mode)));
    }

    Func<string?, Task<List<WeatherModelCelsius>>> accessor;
    
    if (!_accessors.TryGetValue(mode, out accessor))
    {
      throw new ArgumentOutOfRangeException(nameof(mode), $"Unknown access mode: {mode}");
    }

    try
    {
      var result = await accessor(argument).ConfigureAwait(false);
      _logger.LogInformation("Retrieved {Count} weather records", result.Count);
      return Result<List<WeatherModelCelsius>>.Success(result);
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error retrieving weather data via {AccessMode}", mode);
      return Result<List<WeatherModelCelsius>>.Failure(e);
    }
  }
}