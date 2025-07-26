using Microsoft.Extensions.Logging;
using TestLibrary.S1699.Models;

namespace TestLibrary.S1699.Accessor;

public class WeatherApiAccessor: WeatherAccessorBase
{
  public WeatherApiAccessor(ILogger<WeatherApiAccessor> logger) 
    : base(logger)
  {

  }

  public override Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    throw new NotImplementedException();
  }
}
