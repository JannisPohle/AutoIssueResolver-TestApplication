using System.Text;

namespace TestLibrary.S2930;

/// <summary>
/// Implements code smell S2930
/// </summary>
public class FileAccess
{
  public virtual string ReadFromFile(string filePath)
  {
    var fs = new FileStream(filePath, FileMode.Open);
    var content = new byte[fs.Length];
    var bytesRead = 0;
    while (bytesRead < fs.Length)
    {
      bytesRead += fs.Read(content, bytesRead, content.Length - bytesRead);
    }

    return Encoding.UTF8.GetString(content);
  }
}