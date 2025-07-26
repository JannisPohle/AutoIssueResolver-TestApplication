using System.Text.Json.Serialization;

namespace TestLibrary.S1699.Models;

public class WeatherModelCelsius: WeatherModelBase
{
  public int Temperature { get; }

  public override string Unit { get; protected set; }

  [JsonConstructor]
  public WeatherModelCelsius(int temperature)
  {
    Temperature = temperature;
    SetUnit();
  }

  protected override void SetUnit()
  {
    Unit = "Celsius";
  }
}
