using TestLibrary.S4136.Models;

namespace TestLibrary.S4136.Abstractions;

public interface IWeatherOrchestrator
{
  Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessMode mode, string? argument = null);
  Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessMode mode, string? argument, bool throwOnError);
}