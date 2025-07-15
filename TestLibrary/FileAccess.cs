namespace TestLibrary;

/// <summary>
/// Implements code smell S2930
/// </summary>
public class FileAccess
{
  public virtual string ReadFromFile(string filePath)
  {
    var fs = new FileStream(filePath, FileMode.Open);
    var streamReader = new StreamReader(fs);
    return streamReader.ReadToEnd();
  }
}