using Microsoft.Extensions.Logging;
using TestLibrary.S110.Models;

namespace TestLibrary.S110.Accessor;

public abstract class LoggerBase
{
  protected ILogger Logger { get; }

  protected LoggerBase(ILogger logger)
  {
    Logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }
}

public abstract class WeatherAccessorBase
{
  protected ILogger Logger { get; }

  protected WeatherAccessorBase(ILogger logger)
  {
    Logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public abstract Task<List<WeatherModelCelsius>> GetWeather(string? argument);
}
