using Microsoft.Extensions.Logging;
using TestLibrary.S4015.Models;

namespace TestLibrary.S4015.Accessor;

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

  protected static void ValidateArgument(string? argument)
  {
    if (argument == null)
    {
      return;
    }

    if (argument.Trim().Length == 0)
    {
      throw new ArgumentException("Argument cannot be an empty string.", nameof(argument));
    }

    if (argument.Length > 100)
    {
      throw new ArgumentException("Argument cannot exceed 100 characters.", nameof(argument));
    }
  }

  #endregion
}
