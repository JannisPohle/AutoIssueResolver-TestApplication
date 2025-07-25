namespace TestLibrary.S1699.Models;

public abstract class WeatherModelBase
{
  public DateOnly Date { get; } = DateOnly.FromDateTime(DateTime.UtcNow);

  public abstract string Unit { get; }

  protected WeatherModelBase()
  {
  }

  protected abstract void SetUnit();
}
