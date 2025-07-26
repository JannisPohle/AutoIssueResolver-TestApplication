namespace TestLibrary.S1699.Models;

public abstract class WeatherModelBase
{
  public DateOnly Date { get; } = DateOnly.FromDateTime(DateTime.UtcNow);

  public abstract string Unit { get; protected set; }

  protected WeatherModelBase()
  {
    // SetUnit();
  }

  protected virtual void SetUnit()
  {
    // Method implementation can be provided in derived classes
  }
}