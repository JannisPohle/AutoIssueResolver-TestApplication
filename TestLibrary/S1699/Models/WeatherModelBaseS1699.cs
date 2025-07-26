namespace TestLibrary.S1699.Models;

public abstract class WeatherModelBase
{
  public DateOnly Date { get; } = DateOnly.FromDateTime(DateTime.UtcNow);

  public abstract string Unit { get; protected set; }

  protected WeatherModelBase()
  {
    // Constructor no longer calls overridable method to avoid unexpected behavior
  }

  protected abstract void SetUnit();
}