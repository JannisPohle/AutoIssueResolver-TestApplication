using TestLibrary.S2306.Models;

namespace TestLibrary.S2306.Abstractions;

public interface IWeatherOrchestrator
{
  Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessMode mode, string? argument = null);
}
