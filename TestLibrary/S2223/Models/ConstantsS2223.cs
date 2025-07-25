namespace TestLibrary.S2223.Models;

public static class Constants
{
  private static string _unit = "Celsius";
  private static readonly object _unitLock = new object();

  public static string Unit
  {
    get
    {
      lock (_unitLock)
      {
        return _unit;
      }
    }
    set
    {
      lock (_unitLock)
      {
        _unit = value;
      }
    }
  }
}