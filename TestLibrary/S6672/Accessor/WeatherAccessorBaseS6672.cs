using Microsoft.Extensions.Logging;
using TestLibrary.S6672.Models;

namespace TestLibrary.S6672.Accessor;

public abstract class WeatherAccessorBase
{
  #region Properties

  protected ILogger Logger { get; }

  #endregion

  #region Constructors

  protected WeatherAccessorBase(ILogger logger)
  {
    Logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  #endregion

  #region Methods

  public abstract Task<List<WeatherModelCelsius>> GetWeather(string? argument);

  #endregion
}