using System.Text;

namespace TestLibrary;

/// <summary>
/// Implements code smell S2931
/// </summary>
public class FileAccessS2931
{
  private FileStream _fileStream; //TODO maybe use a different IDisposable, since this is exactly the type in the example

  public void OpenWeatherForecastFile()
  {
    _fileStream = new FileStream("TestFiles/WeatherForecast.json", FileMode.Open);
  }

  public string ReadWeatherForecastFile()
  {
    if (_fileStream == null)
    {
      throw new InvalidOperationException("File stream is not initialized. Call OpenWeatherForecastFile first.");
    }

    var content = new byte[_fileStream.Length];
    var bytesRead = 0;
    while (bytesRead < _fileStream.Length)
    {
      bytesRead += _fileStream.Read(content, bytesRead, content.Length - bytesRead);
    }

    return Encoding.UTF8.GetString(content);
  }

  public void CloseWeatherForecastFile()
  {
    _fileStream?.Close();
  }
}