namespace TestLibrary.S2223.Models;

public static class Constants
{
  private static string _unit = "Celsius";
  public static string Unit { get { return _unit; } }
  public static void SetUnit(string newUnit)
  {
    _unit = newUnit;
  }
}
