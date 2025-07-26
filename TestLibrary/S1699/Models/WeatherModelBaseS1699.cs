using System.Text; 
using System.Text.Json; 
using Microsoft.Extensions.Logging; 
using TestLibrary.S1699.Models;

namespace TestLibrary.S1699.Accessor;

public class WeatherFileAccessor: WeatherAccessorBase
{
  public WeatherFileAccessor(ILogger<WeatherFileAccessor> logger) 
    : base(logger)
  {

  }

  /// <inheritdoc />
  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    var stringContent = await ReadFromFile(argument ?? "TestFiles/WeatherModelCelsiusS1699.cs" );
    JsonSerializer.Deserialize<WeatherModelCelsius>(stringContent);
    
    return await Task.FromResult(new List<WeatherModelCelsius>());
  }
}
