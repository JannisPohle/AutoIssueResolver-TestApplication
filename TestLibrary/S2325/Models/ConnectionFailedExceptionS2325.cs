namespace TestLibrary.S2325.Models;

public class ConnectionFailedException: Exception
{
  #region Constructors

  public ConnectionFailedException(string message)
    : base(message)
  { }

  public ConnectionFailedException(string message, Exception innerException)
    : base(message, innerException)
  { }

  #endregion
}
