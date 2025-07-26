namespace TestLibrary.S2365.Models.External;

public class WeatherApiModelResult
{
  #region Members

  private readonly List<WeatherApiModel> _items;

  #endregion

  #region Constructors

  [JsonConstructor]
  public WeatherApiModelResult(List<WeatherApiModel>? items)
  {
    _items = items;
  }

  #endregion

  #region Properties

  // Ensure that the list cannot be modified, by providing a method instead of a property.
  public IEnumerable<WeatherApiModel> GetItems()
  {
    return _items ?? Enumerable.Empty<WeatherApiModel>();
  }

  #endregion
}