using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S1994.Models;
using TestLibrary.S1994.Models.External;

namespace TestLibrary.S1994.Accessor;

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

      var response = await _httpClient.GetFromJsonAsync<List<Models.External.WeatherApiModel>>(url);

      if (response is null)
      {
        Logger.LogWarning("No weather data found for argument: {Argument}", argument);

        throw new DataNotFoundException($"No weather data found for argument: {argument}.");
      }

      var weatherData = new List<WeatherModelCelsius>();

      for (int i = 0; i < response.Count; i++)
      {
        weatherData.Add(new WeatherModelCelsius((int) response[i].Temperature));
      }

      Logger.LogInformation("Found {WeatherDataCount} weather data for location {Argument}.", weatherData.Count, argument);

      return weatherData;
    }
    catch (Exception e)
    {
      Logger.LogWarning(e, "Failed to get weather data with argument: {Argument}", argument);

      throw new ConnectionFailedException($"Failed to connect to the weather API with argument: {argument}.", e);
    }
  }

  private void Dispose(bool disposing)
  {
    if (disposing)
    {
      _httpClient.Dispose();
    }
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  #endregion
}
