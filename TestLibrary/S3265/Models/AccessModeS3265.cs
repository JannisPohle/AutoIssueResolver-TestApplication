namespace TestLibrary.S3265.Models;

[Flags]
public enum AccessModes
{
  None = 1,
  File = 2,
  Mock = 4,
  Database = 8,
  Web = 16,
}
