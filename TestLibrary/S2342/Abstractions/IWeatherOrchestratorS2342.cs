using TestLibrary.S2342.Models;

namespace TestLibrary.S2342.Abstractions;

public interface IWeatherOrchestrator
{
  Task<Result<List<WeatherModelCelsius>>> GetWeather(int mode, string? argument = null);
}
