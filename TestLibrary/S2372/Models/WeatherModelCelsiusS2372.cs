using System.Text.Json.Serialization;

namespace TestLibrary.S2372.Models;

public class WeatherModelCelsius: WeatherModelBase
{
  private readonly int? _temperature;

  public int Temperature
  {
    get
    {
      return GetTemperature();
    }
  }

  public override string Unit => "Celsius";

  [JsonConstructor]
  public WeatherModelCelsius(int temperature)
  {
    _temperature = temperature;
  }

  private int GetTemperature()
  {
    if (_temperature == null || _temperature < -273 || _temperature > 100)
    {
      throw new ArgumentException("Temperature must not be null and be between -273°C and 100°C.");
    }

    return _temperature.Value;
  }
}