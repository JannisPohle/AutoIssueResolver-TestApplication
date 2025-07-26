namespace TestLibrary.S3925.Models;

[Serializable]
public class DataNotFoundException: Exception, System.Runtime.Serialization.ISerializable
{
  public string? Argument { get; init; }

  public DataNotFoundException(string? message)
    : base(message)
  { }

  public DataNotFoundException(string? message, Exception? innerException)
    : base(message, innerException)
  { }

  protected DataNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    : base(info, context)
  {
    Argument = info.GetString(nameof(Argument));
  }

  public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
  {
    base.GetObjectData(info, context);
    info.AddValue(nameof(Argument), Argument);
  }
}
