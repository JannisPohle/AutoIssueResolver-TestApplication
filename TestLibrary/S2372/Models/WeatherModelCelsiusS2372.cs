using System.Text.Json.Serialization;

namespace TestLibrary.S2372.Models;

public class WeatherModelCelsius: WeatherModelBase
{
  private readonly int _temperature;

  public int Temperature => _temperature;

  public override string Unit => "Celsius";

  [JsonConstructor]
  public WeatherModelCelsius(int temperature)
  {
    if (temperature < -273 || temperature > 100) throw new ArgumentException("Temperature must be between -273°C and 100°C.", nameof(temperature));
    _temperature = temperature;
  }
}
