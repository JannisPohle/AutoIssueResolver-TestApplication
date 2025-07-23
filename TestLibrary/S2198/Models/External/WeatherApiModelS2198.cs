namespace TestLibrary.S2198.Models.External;

public class WeatherApiModel
{
  #region Properties

  public string Location { get; set; } = string.Empty;

  public float Temperature { get; set; }

  public string Condition { get; set; } = string.Empty;

  #endregion
}
