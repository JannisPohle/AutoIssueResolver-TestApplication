using TestLibrary.S2275.Models;

namespace TestLibrary.S2275.Abstractions;

public interface IWeatherOrchestrator
{
  Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessMode mode, string? argument = null);
}
