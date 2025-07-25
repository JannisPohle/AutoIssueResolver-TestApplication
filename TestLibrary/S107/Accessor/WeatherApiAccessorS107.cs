using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S107.Models;

namespace TestLibrary.S107.Accessor;

public sealed class WeatherApiAccessor(ILogger<WeatherApiAccessor> logger): WeatherAccessorBase(logger), IDisposable
{
  #region Members

  private readonly HttpClient _httpClient = new();

  #endregion

  #region Methods

  public async Task<List<WeatherModelCelsius>> GetWeather(
    string? location,
    string? startTime,
    string? endTime,
    string? longitude,
    string? latitude,
    string? unit
  )
  {
    var weatherRequest = new WeatherRequest(location, startTime, endTime, longitude, latitude, unit);
    return await GetWeather(weatherRequest);
  }

  public async Task<List<WeatherModelCelsius>> GetWeather(WeatherRequest weatherRequest)
  {
    try
    {
      var url = "http://localhost:31246/v1/api/weather";

      Dictionary<string, string> queryParams = new();
      if (!string.IsNullOrWhiteSpace(weatherRequest.Location))
      {
        queryParams.Add("location", weatherRequest.Location);
      }

      if (!string.IsNullOrWhiteSpace(weatherRequest.StartTime))
      {
        queryParams.Add("startTime", weatherRequest.StartTime);
      }

      if (!string.IsNullOrWhiteSpace(weatherRequest.EndTime))
      {
        queryParams.Add("endTime", weatherRequest.EndTime);
      }

      if (!string.IsNullOrWhiteSpace(weatherRequest.Longitude))
      {
        queryParams.Add("longitude", weatherRequest.Longitude);
      }

      if (!string.IsNullOrWhiteSpace(weatherRequest.Latitude))
      {
        queryParams.Add("latitude", weatherRequest.Latitude);
      }

      if (!string.IsNullOrWhiteSpace(weatherRequest.Unit))
      {
        queryParams.Add("unit", weatherRequest.Unit);
      }

      var queryUrl = string.Empty;
      if (queryParams.Count > 0)
      {
        queryUrl = "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
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
        weatherData.Add(new WeatherModelCelsius((int) (weatherModel?.Temperature ?? 0)));
      }

      Logger.LogInformation("Found {WeatherDataCount} weather data for arguments: {Query}.", weatherData.Count, queryUrl);

      return weatherData;
    }
    catch (Exception e)
    {
      Logger.LogWarning(e, "Failed to get weather data with arguments:  {Location}, {StartTime}, {EndTime}, {Longitude}, {Latitude}, {Unit}", weatherRequest.Location, weatherRequest.StartTime, weatherRequest.EndTime, weatherRequest.Longitude, weatherRequest.Latitude, weatherRequest.Unit);

      throw new ConnectionFailedException($"Failed to connect to the weather API with arguments: {weatherRequest.Location}, {weatherRequest.StartTime}, {weatherRequest.EndTime}, {weatherRequest.Longitude}, {weatherRequest.Latitude}, {weatherRequest.Unit}.", e);
    }
  }

  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    WeatherRequest weatherRequest = new()
    {
      Location = argument
    };
    return await GetWeather(weatherRequest);
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

public class WeatherRequest
{
  public string? Location { get; set; }
  public string? StartTime { get; set; }
  public string? EndTime { get; set; }
  public string? Longitude { get; set; }
  public string? Latitude { get; set; }
  public string? Unit { get; set; }

  public WeatherRequest(string? location = null, string? startTime = null, string? endTime = null, string? longitude = null, string? latitude = null, string? unit = null)
  {
    Location = location;
    StartTime = startTime;
    EndTime = endTime;
    Longitude = longitude;
    Latitude = latitude;
    Unit = unit;
  }
}
