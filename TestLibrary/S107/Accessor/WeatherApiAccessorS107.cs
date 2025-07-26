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

  private static Dictionary<string, string> BuildQueryParams(WeatherApiParams parameters)
  {
    var queryParams = new Dictionary<string, string>();
    if (!string.IsNullOrWhiteSpace(parameters.Location))
    {
      queryParams.Add("location", parameters.Location);
    }

    if (!string.IsNullOrWhiteSpace(parameters.StartTime))
    {
      queryParams.Add("startTime", parameters.StartTime);
    }

    if (!string.IsNullOrWhiteSpace(parameters.EndTime))
    {
      queryParams.Add("endTime", parameters.EndTime);
    }

    if (!string.IsNullOrWhiteSpace(parameters.Longitude))
    {
      queryParams.Add("longitude", parameters.Longitude);
    }

    if (!string.IsNullOrWhiteSpace(parameters.Latitude))
    {
      queryParams.Add("latitude", parameters.Latitude);
    }

    if (!string.IsNullOrWhiteSpace(parameters.Unit))
    {
      queryParams.Add("unit", parameters.Unit);
    }
    return queryParams;
  }

  private static string BuildQueryUrl(Dictionary<string, string> queryParams)
  {
    if (queryParams.Count == 0) return string.Empty;
    return "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
  }

  public async Task<List<WeatherModelCelsius>> GetWeather(WeatherApiParams parameters)
  {
    try
    {
      var url = "http://localhost:31246/v1/api/weather";
      var queryParams = BuildQueryParams(parameters);
      var queryUrl = BuildQueryUrl(queryParams);

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
      Logger.LogWarning(e, "Failed to get weather data with arguments:  {Location}, {StartTime}, {EndTime}, {Longitude}, {Latitude}, {Unit}", 
        parameters.Location, parameters.StartTime, parameters.EndTime, parameters.Longitude, parameters.Latitude, parameters.Unit);

      throw new ConnectionFailedException($"Failed to connect to the weather API with arguments: {parameters.Location}, {parameters.StartTime}, {parameters.EndTime}, {parameters.Longitude}, {parameters.Latitude}, {parameters.Unit}.", e);
    }
  }

  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    return await GetWeather(new WeatherApiParams { Location = argument });
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

public class WeatherApiParams
{
    public string? Location { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public string? Longitude { get; set; }
    public string? Latitude { get; set; }
    public string? Unit { get; set; }
}