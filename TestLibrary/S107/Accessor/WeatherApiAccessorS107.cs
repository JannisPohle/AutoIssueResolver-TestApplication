using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S107.Models;

namespace TestLibrary.S107.Accessor;

public record WeatherApiRequest(string? Location, string? StartTime, string? EndTime, string? Longitude, string? Latitude, string? Unit);

public sealed class WeatherApiAccessor(ILogger<WeatherApiAccessor> logger): WeatherAccessorBase(logger), IDisposable
{
  #region Members

  private readonly HttpClient _httpClient = new();

  #endregion

  #region Methods

  public async Task<List<WeatherModelCelsius>> GetWeather(WeatherApiRequest request)
  {
    try
    {
      var url = "http://localhost:31246/v1/api/weather";

      Dictionary<string, string> queryParams = new();
      if (!string.IsNullOrWhiteSpace(request.Location))
      {
        queryParams.Add("location", request.Location);
      }

      if (!string.IsNullOrWhiteSpace(request.StartTime))
      {
        queryParams.Add("startTime", request.StartTime);
      }

      if (!string.IsNullOrWhiteSpace(request.EndTime))
      {
        queryParams.Add("endTime", request.EndTime);
      }

      if (!string.IsNullOrWhiteSpace(request.Longitude))
      {
        queryParams.Add("longitude", request.Longitude);
      }

      if (!string.IsNullOrWhiteSpace(request.Latitude))
      {
        queryParams.Add("latitude", request.Latitude);
      }

      if (!string.IsNullOrWhiteSpace(request.Unit))
      {
        queryParams.Add("unit", request.Unit);
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
      Logger.LogWarning(e, "Failed to get weather data with arguments:  {Location}, {StartTime}, {EndTime}, {Longitude}, {Latitude}, {Unit}", request.Location, request.StartTime, request.EndTime, request.Longitude, request.Latitude, request.Unit);

      throw new ConnectionFailedException($"Failed to connect to the weather API with arguments: {request.Location}, {request.StartTime}, {request.EndTime}, {request.Longitude}, {request.Latitude}, {request.Unit}.", e);
    }
  }

  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    return await GetWeather(new WeatherApiRequest(argument, null, null, null, null, null));
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
