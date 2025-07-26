namespace TestLibrary.S3442.Models;

public abstract class WeatherModelBase
{
  public DateOnly Date { get; }

  public abstract string Unit { get; }

  protected WeatherModelBase()
  {
    Date = DateOnly.FromDateTime(DateTime.Now);
  }
}
