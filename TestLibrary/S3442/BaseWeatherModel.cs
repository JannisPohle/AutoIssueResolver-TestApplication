using System.Runtime.InteropServices.JavaScript;

namespace TestLibrary.S3442;

public abstract class BaseWeatherModel
{
  public DateOnly Date { get; }

  public BaseWeatherModel()
  {
    Date = DateOnly.FromDateTime(DateTime.UtcNow);
  }
}