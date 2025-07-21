using TestLibrary.S2223.Models;

namespace TestLibrary.S2223.Abstractions;

public interface IWeatherOrchestrator
{
  Task<Result<List<WeatherModel>>> GetWeather(AccessMode mode, string? argument = null);

  Task ChangeUnit(string newUnit);
}
