namespace TestLibrary.S2346.Models;

[Flags]
public enum AccessModes
{
  Nothing = 0,
  File = 1,
  Mock = 2,
  Database = 4,
  Web = 8,
}
