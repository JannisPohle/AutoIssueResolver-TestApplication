using Microsoft.Extensions.Logging;
using TestLibrary.S2223.Models;

namespace TestLibrary.S2223.Accessor;

public class WeatherApiAccessor: WeatherAccessorBase
{
  public WeatherApiAccessor(ILogger<WeatherApiAccessor> logger)
    : base(logger)
  { }

  public override Task<List<WeatherModel>> GetWeather(string? argument)
  {
    throw new NotImplementedException();
  }
}
