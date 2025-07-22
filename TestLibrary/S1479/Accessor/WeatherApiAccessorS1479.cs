using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S1479.Models;
using TestLibrary.S1479.Models.External;

namespace TestLibrary.S1479.Accessor;

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

      var response = _httpClient.GetFromJsonAsAsyncEnumerable<WeatherApiModel>(url);

      if (response is null)
      {
        Logger.LogWarning("No weather data found for argument: {Argument}", argument);

        throw new DataNotFoundException($"No weather data found for argument: {argument}.");
      }

      var weatherData = new List<WeatherModelCelsius>();

      await foreach (var weatherModel in response)
      {
        weatherData.Add(new WeatherModelCelsius((int) weatherModel.Temperature));
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
