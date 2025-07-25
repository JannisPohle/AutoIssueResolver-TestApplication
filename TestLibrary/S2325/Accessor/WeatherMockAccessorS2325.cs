using Microsoft.Extensions.Logging;
using TestLibrary.S2325.Models;

namespace TestLibrary.S2325.Accessor;

public class WeatherMockAccessor: WeatherAccessorBase
{
  public WeatherMockAccessor(ILogger<WeatherMockAccessor> logger)
    : base(logger)
  { }

  public override Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    var weather = GenerateWeatherData(argument);

    if (!weather.Any())
    {
      throw new DataNotFoundException(