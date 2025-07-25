using System.Text.Json.Serialization;

namespace TestLibrary.S2365.Models.External;

public class WeatherApiModelResult
{
  #region Members

  private readonly List<WeatherApiModel> _items;

  #endregion

  #region Properties

  public List<WeatherApiModel> Items => _items ?? [];

  #endregion

  #region Constructors

  [JsonConstructor]
  public WeatherApiModelResult(List<WeatherApiModel>? items)
  {
    _items = items;
  }

  #endregion
}