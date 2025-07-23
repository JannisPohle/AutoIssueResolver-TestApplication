namespace TestLibrary.S3358.Models;

public class DataNotFoundException: Exception
{
  public DataNotFoundException(string? message)
    : base(message)
  { }

  public DataNotFoundException(string? message, Exception? innerException)
    : base(message, innerException)
  { }
}
