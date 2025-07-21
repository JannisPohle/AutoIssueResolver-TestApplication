namespace TestLibrary.S4015.Models;

public class Result<TResult>
  where TResult : class
{
  #region Properties

  public TResult? Payload { get; init; }

  public Exception? Exception { get; init; }

  public bool IsSuccess => Exception == null;

  #endregion

  #region Constructors

  private Result()
  { }

  #endregion

  #region Methods

  public static Result<TResult> Success(TResult payload)
  {
    return new Result<TResult>
    {
      Payload = payload,
      Exception = null,
    };
  }

  public static Result<TResult> Failure(Exception exception)
  {
    return new Result<TResult>
    {
      Payload = null,
      Exception = exception,
    };
  }

  #endregion
}
