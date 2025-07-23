namespace TestLibrary.S2681.Models.External;

public class WeatherApiModel
{
  #region Properties

  public string Location { get; set; } = string.Empty;

  public double Temperature { get; set; }

  public string Condition { get; set; } = string.Empty;

  #endregion
}
