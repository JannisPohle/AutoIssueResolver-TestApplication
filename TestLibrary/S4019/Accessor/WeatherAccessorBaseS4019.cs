using Microsoft.Extensions.Logging;
using TestLibrary.S4019.Models;

namespace TestLibrary.S4019.Accessor;

public abstract class WeatherAccessorBase
{
  #region Properties

  protected ILogger<WeatherAccessorBase> Logger { get; }

  #endregion

  #region Constructors

  protected WeatherAccessorBase(ILogger<WeatherAccessorBase> logger)
  {
    Logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  #endregion

  #region Methods

  public abstract Task<List<WeatherModelCelsius>> GetWeather(string? argument);

  public bool ValidateWeatherData(WeatherModelCelsius data)
  {
    var success = data.Temperature is > -30 and < 120;
    Logger.LogTrace("Validate weather data : {Success}", success);

    return success;
  }

  #endregion
}
