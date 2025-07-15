namespace TestLibrary.S3442;

public class WeatherCelsius: BaseWeatherModel
{
  public int Temperature { get; }

  public WeatherCelsius(int temperature)
  {
    Temperature = temperature;
  }
}