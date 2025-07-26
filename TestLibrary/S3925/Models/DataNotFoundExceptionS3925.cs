using System.Runtime.Serialization;

namespace TestLibrary.S3925.Models;

[Serializable]
public class DataNotFoundException: Exception, ISerializable
{
  public string? Argument { get; init; }

  public DataNotFoundException(string? message)
    : base(message)
  { }

  public DataNotFoundException(string? message, Exception? innerException)
    : base(message, innerException)
  { }

  protected DataNotFoundException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
    Argument = info.GetString(nameof(Argument));
  }

  public override void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    base.GetObjectData(info, context);
    info.AddValue(nameof(Argument), Argument);
  }
}