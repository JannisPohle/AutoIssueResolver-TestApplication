using Microsoft.Extensions.Logging;
using TestLibrary.S2223.Models;

namespace TestLibrary.S2223.Accessor;

public class WeatherDbAccessor: WeatherAccessorBase
{
  public WeatherDbAccessor(ILogger<WeatherDbAccessor> logger)
    : base(logger)
  { }

  public override Task<List<WeatherModel>> GetWeather(string? argument)
  {
    throw new NotImplementedException();
  }
}
