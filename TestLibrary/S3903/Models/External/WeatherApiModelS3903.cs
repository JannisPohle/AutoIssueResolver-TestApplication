namespace TestLibrary.S3903.Models;

public class WeatherApiModel
{
  #region Properties

  public string Location { get; set; } = string.Empty;

  public double Temperature { get; set; }

  public string Condition { get; set; } = string.Empty;

  #endregion
}