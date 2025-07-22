namespace TestLibrary.S2372.Models;

public abstract class WeatherModelBase
{
  public DateOnly Date { get; } = DateOnly.FromDateTime(DateTime.UtcNow);

  public abstract string Unit { get; }
}
