namespace TestLibrary.S2223.Models;

public static class Constants
{
  private static string _unit = "Celsius";
  public static string Unit
  {
    get => _unit;
    internal set => _unit = value;
  }
}