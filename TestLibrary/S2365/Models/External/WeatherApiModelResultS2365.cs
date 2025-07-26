using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace TestLibrary.S2365.Models.External;

public class WeatherApiModelResult
{
  #region Members

  private readonly List<WeatherApiModel> _items;

  #endregion

  #region Properties

  // Provide method to get a copy to avoid unexpected performance cost in property
  public List<WeatherApiModel> GetItems() => _items?.ToList() ?? new List<WeatherApiModel>();

  #endregion

  #region Constructors

  [JsonConstructor]
  public WeatherApiModelResult(List<WeatherApiModel>? items)
  {
    _items = items;
  }

  #endregion
}