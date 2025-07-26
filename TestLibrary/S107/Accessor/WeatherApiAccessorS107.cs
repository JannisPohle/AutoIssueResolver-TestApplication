using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S107.Models;

namespace TestLibrary.S107.Accessor;

public readonly record struct WeatherApiQuery(string? Location, string? StartTime, string? EndTime, string? Longitude, string? Latitude, string? Unit);

public sealed class WeatherApiAccessor(ILogger<WeatherApiAccessor> logger): WeatherAccessorBase(logger), IDisposable
{
  #region Members

  private readonly HttpClient _httpClient = new();

  #endregion

  #region Methods

  public async Task<List<WeatherModelCelsius>> GetWeather(WeatherApiQuery query)
  {
    try
    {
      var url = "http://localhost:31246/v1/api/weather";

      Dictionary<string, string> queryParams = new();
      if (!string.IsNullOrWhiteSpace(query.Location))
      {
        queryParams.Add("location", query.Location);
      }

      if (!string.IsNullOrWhiteSpace(query.StartTime))
      {
        queryParams.Add("startTime", query.StartTime);
      }

      if (!string.IsNullOrWhiteSpace(query.EndTime))
      {
        queryParams.Add("endTime", query.EndTime);
      }

      if (!string.IsNullOrWhiteSpace(query.Longitude))
      {
        queryParams.Add("longitude", query.Longitude);
      }

      if (!string.IsNullOrWhiteSpace(query.Latitude))
      {
        queryParams.Add("latitude", query.Latitude);
      }

      if (!string.IsNullOrWhiteSpace(query.Unit))
      {
        queryParams.Add("unit", query.Unit);
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
        weatherData.Add(new WeatherModelCelsius((int)(weatherModel?.Temperature ?? 0)));
      }

      Logger.LogInformation("Found {WeatherDataCount} weather data for arguments: {Query}.", weatherData.Count, queryUrl);

      return weatherData;
    }
    catch (Exception e)
    {
      Logger.LogWarning(e, "Failed to get weather data with arguments:  {Location}, {StartTime}, {EndTime}, {Longitude}, {Latitude}, {Unit}", query.Location, query.StartTime, query.EndTime, query.Longitude, query.Latitude, query.Unit);

      throw new ConnectionFailedException($"Failed to connect to the weather API with arguments: {query.Location}, {query.StartTime}, {query.EndTime}, {query.Longitude}, {query.Latitude}, {query.Unit}.", e);
    }
  }

  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    var arguments = argument?.Split(';') ?? [];
    var query = new WeatherApiQuery(arguments.ElementAtOrDefault(0), arguments.ElementAtOrDefault(1), arguments.ElementAtOrDefault(2), arguments.ElementAtOrDefault(3), arguments.ElementAtOrDefault(4), arguments.ElementAtOrDefault(5));
    return await GetWeather(query);
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
