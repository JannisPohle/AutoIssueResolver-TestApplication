namespace TestLibrary.S3925.Models;

[Serializable]
public class DataNotFoundException: Exception
{
  public string? Argument { get; init; }

  public DataNotFoundException(string? message)
    : base(message)
  { }

  public DataNotFoundException(string? message, Exception? innerException)
    : base(message, innerException)
  { }
}
