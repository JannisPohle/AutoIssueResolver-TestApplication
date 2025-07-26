// New code block
public class WeatherModelCelsius : WeatherModelBase
{
  private readonly int? _temperature;

  public int Temperature
  {
    get
    {
      if (_temperature == null || _temperature < -273 || _temperature > 100)
      {
        throw new ArgumentException("Temperature must not be null and be between -273°C and 100°C.");
      }

      return _temperature.Value;
    }
  }

  public override string Unit => "Celsius";

  [JsonConstructor]
  public WeatherModelCelsius(int temperature)
  {
    _temperature = temperature;
  }
}
// End of new code block