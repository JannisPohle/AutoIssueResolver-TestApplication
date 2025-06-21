namespace TestLibrary;

public class Disposable: IDisposable
{
  private object? _largeObject = new();

  public void Dispose()
  {
    _largeObject = null;
  }
}