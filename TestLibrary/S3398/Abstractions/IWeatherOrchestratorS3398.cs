using TestLibrary.S3398.Models;

namespace TestLibrary.S3398.Abstractions;

public interface IWeatherOrchestrator
{
  Task<Result<List<WeatherModelCelsius>>> GetWeather(AccessMode mode, string? argument = null);
}
