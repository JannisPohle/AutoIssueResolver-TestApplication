using System.Text.Json.Serialization;

namespace TestLibrary.Template.Models;

public class WeatherModelCelsius: WeatherModelBase
{
  public int Temperature { get; }
  public override string Unit => "Celsius";

  [JsonConstructor]
  public WeatherModelCelsius(int temperature)
  {
    Temperature = temperature;
  }
}