public sealed class WeatherApiAccessor(ILogger<WeatherApiAccessor> logger): WeatherAccessorBase(logger), IDisposable
{
  #region Members

  private readonly HttpClient _httpClient = new();

  #endregion

  #region Methods

  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    try
    {
      var url = "http://localhost:31246/v1/api/weather";

      if (!string.IsNullOrWhiteSpace(argument))
      {
        url += $"?location={argument}";
      }

      var response = _httpClient.GetFromJsonAsAsyncEnumerable<Models.External.WeatherApiModel>(url);

      if (response is null)
      {
        throw new InvalidOperationException("Failed to fetch data from the API.");
      }

      return await response.Select(x => new WeatherModelCelsius((int)x.Temperature)).ToListAsync();
    }
    catch (Exception e)
    {
      throw new ConnectionFailedException("Error fetching weather data from web service", e);
    }
  }

  #endregion
}
