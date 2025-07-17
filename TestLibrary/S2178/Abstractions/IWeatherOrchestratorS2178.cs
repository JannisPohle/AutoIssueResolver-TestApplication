using TestLibrary.S2178.Models;

namespace TestLibrary.S2178.Abstractions;

public interface IWeatherOrchestrator
{
  Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessMode mode, string? argument = null);
}
