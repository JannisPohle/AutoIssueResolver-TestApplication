using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TestLibrary.S2365.Models.External;

public class WeatherApiModelResult
{
  #region Members

  private readonly List<WeatherApiModel> _items;

  #endregion

  #region Properties

  //Expose the items as a read-only collection without creating a copy
  public IReadOnlyList<WeatherApiModel> Items => _items?.AsReadOnly() ?? Array.Empty<WeatherApiModel>();

  #endregion

  #region Constructors

  [JsonConstructor]
  public WeatherApiModelResult(List<WeatherApiModel>? items)
  {
    _items = items ?? [];
  }

  #endregion
}