using TestLibrary.S2931.Models;

namespace TestLibrary.S2931.Abstractions;

public interface IWeatherOrchestrator
{
  Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessMode mode, string? argument = null);
}
