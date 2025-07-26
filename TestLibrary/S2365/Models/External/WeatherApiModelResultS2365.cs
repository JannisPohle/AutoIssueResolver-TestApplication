using System.Text.Json.Serialization;

namespace TestLibrary.S2365.Models.External;

public class WeatherApiModelResult
{
  #region Members

  private readonly List<WeatherApiModel> _items;

  #endregion

  #region Properties

  //Ensure that the list cannot be modified, by creating a copy of the list
  public IEnumerable<WeatherApiModel> GetItems() => _items?.ToList() ?? [];

  #endregion

  #region Constructors

  [JsonConstructor]
  public WeatherApiModelResult(List<WeatherApiModel>? items)
  {
    _items = items;
  }

  #endregion
}