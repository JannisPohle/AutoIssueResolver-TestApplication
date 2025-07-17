using Microsoft.Extensions.Logging;
using TestLibrary.S3265.Models;

namespace TestLibrary.S3265.Accessor;

public class WeatherDbAccessor: WeatherAccessorBase
{
  public WeatherDbAccessor(ILogger<WeatherDbAccessor> logger)
    : base(logger)
  { }

  public override Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    throw new NotImplementedException();
  }
}
