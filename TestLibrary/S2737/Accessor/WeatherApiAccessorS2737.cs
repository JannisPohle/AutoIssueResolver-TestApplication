using Microsoft.Extensions.Logging;
using TestLibrary.S2737.Models;

namespace TestLibrary.S2737.Accessor;

public class WeatherApiAccessor: WeatherAccessorBase
{
  public WeatherApiAccessor(ILogger<WeatherApiAccessor> logger)
    : base(logger)
  { }

  public override Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    throw new NotImplementedException();
  }
}
