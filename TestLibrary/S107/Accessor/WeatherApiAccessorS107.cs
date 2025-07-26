using System;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S107.Models;

namespace TestLibrary.S107.Accessor;

public sealed class WeatherApiAccessor(ILogger<WeatherApiAccessor> logger): WeatherAccessorBase(logger), IDisposable
{
  #region Members

  private readonly HttpClient _httpClient = new();

  #endregion

  public record WeatherQueryParams(string? Location = null, string? StartTime = null, string? EndTime = null, string? Longitude = null, string? Latitude = null, string? Unit = null);

  public async Task<List<WeatherModelCelsius>> GetWeather(WeatherQueryParams queryParams)
  {
    try
    {
      var url = "http://localhost:31246/v1/api/weather";

      var paramsDict = new Dictionary<string, string>();
      if (!string.IsNullOrWhiteSpace(queryParams.Location))
      {
        paramsDict.Add("location", queryParams.Location);
      }

      if (!string.IsNullOrWhiteSpace(queryParams.StartTime))
      {
        paramsDict.Add("startTime", queryParams.StartTime);
      }

      if (!string.IsNullOrWhiteSpace(queryParams.EndTime))
      {
        paramsDict.Add("endTime", queryParams.EndTime);
      }

      if (!string.IsNullOrWhiteSpace(queryParams.Longitude))
      {
        paramsDict.Add("longitude", queryParams.Longitude);
      }

      if (!string.IsNullOrWhiteSpace(queryParams.Latitude))
      {
        paramsDict.Add("latitude", queryParams.Latitude);
      }

      if (!string.IsNullOrWhiteSpace(queryParams.Unit))
      {
        paramsDict.Add("unit", queryParams.Unit);
      }

      var queryUrl = string.Empty;
      if (paramsDict.Count > 0)
      {
        queryUrl = "?" + string.Join("&", paramsDict.Select(kvp => $"{kvp.Key}={kvp.Value}"));
      }

      var response = _httpClient.GetFromJsonAsAsyncEnumerable<Models.External.WeatherApiModel>(url + queryUrl);

      if (response is null)
      {
        Logger.LogWarning("No weather data found for with arguments: {Query}", queryUrl);

        throw new DataNotFoundException($"No weather data found for arguments: {queryUrl}.");
      }

      var weatherData = new List<WeatherModelCelsius>();

      await foreach (var weatherModel in response)
      {
        weatherData.Add(new WeatherModelCelsius((int)(weatherModel?.Temperature ?? 0)));
      }

      Logger.LogInformation("Found {WeatherDataCount} weather data for arguments: {Query}._", weatherData.Count, queryUrl);

      return weatherData;
    }
    catch (Exception e)
    {
      Logger.LogWarning(e, "Failed to get weather data with parameters: {QueryParams}", queryParams);

      throw new ConnectionFailedException($"Failed to connect to the weather API with parameters: {queryParams}.", e);
    }
  }

  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    var parts = argument?.Split(';') ?? Array.Empty<string>();
    var queryParams = new WeatherQueryParams(
      parts.ElementAtOrDefault(0),
      parts.ElementAtOrDefault(1),
      parts.ElementAtOrDefault(2),
      parts.ElementAtOrDefault(3),
      parts.ElementAtOrDefault(4),
      parts.ElementAtOrDefault(5));
    return await GetWeather(queryParams);
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

  #region Members

  #endregion
}