namespace TestLibrary.S1699.Models;

public abstract class WeatherModelBase
{
  public DateOnly Date { get; } = DateOnly.FromDateTime(DateTime.UtcNow);

  public string Unit { get; }

  protected WeatherModelBase(string unit)
  {
    Unit = unit;
  }
}
