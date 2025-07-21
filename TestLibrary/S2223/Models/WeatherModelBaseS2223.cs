using System.Text.Json.Serialization;

namespace TestLibrary.S2223.Models;

public class WeatherModel
{
  public DateOnly Date { get; } = DateOnly.FromDateTime(DateTime.UtcNow);
  public int Temperature { get; }
  public string Unit { get; }

  [JsonConstructor]
  public WeatherModel(int temperature)
  {
    Unit = Constants.Unit;
    Temperature = temperature;
  }
}
