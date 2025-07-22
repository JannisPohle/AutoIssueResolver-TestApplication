using TestLibrary.S1479.Models;

namespace TestLibrary.S1479.Abstractions;

public interface IWeatherOrchestrator
{
  Task<Result<List<WeatherModelCelsius>>> GetWeather(string mode, string? argument = null);
}
