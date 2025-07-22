using Microsoft.Extensions.Logging;
using TestLibrary.S107.Models;

namespace TestLibrary.S107.Accessor;

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

  #endregion
}
