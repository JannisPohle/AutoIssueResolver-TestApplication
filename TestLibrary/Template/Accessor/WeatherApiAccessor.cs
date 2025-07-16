using Microsoft.Extensions.Logging;
using TestLibrary.Template.Models;

namespace TestLibrary.Template.Accessor;

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