using TestLibrary.S2346.Models;

namespace TestLibrary.S2346.Abstractions;

public interface IWeatherOrchestrator
{
  Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessModes mode, string? argument = null);
}
