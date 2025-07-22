using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S107.Models;
using TestLibrary.S107.Models.External;

namespace TestLibrary.S107.Accessor;

public sealed class WeatherApiAccessor(ILogger<WeatherApiAccessor> logger): WeatherAccessorBase(logger), IDisposable
{
  #region Members

  private readonly HttpClient _httpClient = new();

  #endregion

  #region Methods

  public async Task<List<WeatherModelCelsius>> GetWeather(string? location, string? startTime, string? endTime, string? longitude, string? latitude, string? unit)
  {
    try
    {
      var url = "http://localhost:31246/v1/api/weather";

      Dictionary<string, string> queryParams = new();
      if (!string.IsNullOrWhiteSpace(location))
      {
        queryParams.Add("location", location);
      }

      if (!string.IsNullOrWhiteSpace(startTime))
      {
        queryParams.Add("startTime", startTime);
      }

      if (!string.IsNullOrWhiteSpace(endTime))
      {
        queryParams.Add("endTime", endTime);
      }

      if (!string.IsNullOrWhiteSpace(longitude))
      {
        queryParams.Add("longitude", longitude);
      }

      if (!string.IsNullOrWhiteSpace(latitude))
      {
        queryParams.Add("latitude", latitude);
      }

      if (!string.IsNullOrWhiteSpace(unit))
      {
        queryParams.Add("unit", unit);
      }

      var queryUrl = string.Empty;
      if (queryParams.Count > 0)
      {
        queryUrl = "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
      }

      var response = _httpClient.GetFromJsonAsAsyncEnumerable<WeatherApiModel>(url + queryUrl);

      if (response is null)
      {
        Logger.LogWarning("No weather data found for with arguments: {Query}", queryUrl);

        throw new DataNotFoundException($"No weather data found for arguments: {queryUrl}.");
      }

      var weatherData = new List<WeatherModelCelsius>();

      await foreach (var weatherModel in response)
      {
        weatherData.Add(new WeatherModelCelsius((int) (weatherModel?.Temperature ?? 0)));
      }

      Logger.LogInformation("Found {WeatherDataCount} weather data for arguments: {Query}.", weatherData.Count, queryUrl);

      return weatherData;
    }
    catch (Exception e)
    {
      Logger.LogWarning(e, "Failed to get weather data with arguments:  {Location}, {StartTime}, {EndTime}, {Longitude}, {Latitude}, {Unit}", location, startTime, endTime, longitude, latitude, unit);

      throw new ConnectionFailedException($"Failed to connect to the weather API with arguments: {location}, {startTime}, {endTime}, {longitude}, {latitude}, {unit}.", e);
    }
  }

  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    return await GetWeather(argument, null, null, null, null, null);
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
