using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S6672.Models;

namespace TestLibrary.S6672.Accessor;

public sealed class WeatherApiAccessor: WeatherAccessorBase, IDisposable
{
  #region Members

  private readonly HttpClient _httpClient = new();

  #endregion

  #region Constructors

  public WeatherApiAccessor(ILogger<WeatherApiAccessor> logger)
    : base(new LoggerAdapter(logger))
  { }

  private sealed class LoggerAdapter : ILogger<WeatherAccessorBase>
  {
    private readonly ILogger<WeatherApiAccessor> _inner;

    public LoggerAdapter(ILogger<WeatherApiAccessor> inner)
    {
      _inner = inner;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
      return _inner.BeginScope(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
      return _inner.IsEnabled(logLevel);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
      _inner.Log(logLevel, eventId, state, exception, formatter);
    }
  }

  #endregion

  #region Methods

  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    try
    {
      var url = "http://localhost:31246/v1/api/weather";

      if (!string.IsNullOrWhiteSpace(argument))
      {
        url += $"?location={argument}";
      }

      var response = _httpClient.GetFromJsonAsAsyncEnumerable<Models.External.WeatherApiModel>(url);

      if (response is null)
      {
        Logger.LogWarning("No weather data found for argument: {Argument}", argument);

        throw new DataNotFoundException($"No weather data found for argument: {argument}.");
      }

      var weatherData = new List<WeatherModelCelsius>();

      await foreach (var weatherModel in response)
      {
        weatherData.Add(new WeatherModelCelsius((int) weatherModel.Temperature));
      }

      Logger.LogInformation("Found {WeatherDataCount} weather data for location {Argument}.", weatherData.Count, argument);

      return weatherData;
    }
    catch (Exception e)
    {
      Logger.LogWarning(e, "Failed to get weather data with argument: {Argument}", argument);

      throw new ConnectionFailedException($"Failed to connect to the weather API with argument: {argument}.", e);
    }
  }

  private void Dispose(bool disposing)
  {
    if (disposing)
    {
      _httpClient.Dispose();
    }
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  #endregion
}
