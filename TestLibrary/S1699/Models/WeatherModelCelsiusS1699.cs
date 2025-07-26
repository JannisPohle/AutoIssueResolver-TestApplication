using System.Text.Json.Serialization;

namespace TestLibrary.S1699.Models;

public class WeatherModelCelsius: WeatherModelBase
{
  public int Temperature { get; }

  [JsonConstructor]
  public WeatherModelCelsius(int temperature)
    : base("Celsius")
  {
    Temperature = temperature;
  }
}
