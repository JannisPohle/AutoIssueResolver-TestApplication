using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S107.Models;

namespace TestLibrary.S107.Accessor;

public sealed class WeatherApiAccessor : WeatherAccessorBase, IDisposable
{
  #region Members

  private readonly HttpClient _httpClient = new();

  #endregion

  #region Methods

  public async Task<List<WeatherModelCelsius>> GetWeather(WeatherQueryParameters queryParameters)
  {
    try
    {
      var url = "http://localhost:31246/v1/api/weather";

      var queryParams = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrWhiteSpace(queryParameters.Location))
      {
        queryParams.Add(new KeyValuePair<string, string>("location", queryParameters.Location));
      }

      if (!string.IsNullOrWhiteSpace(queryParameters.StartTime))
      {
        queryParams.Add(new KeyValuePair<string, string>("startTime", queryParameters.StartTime));
      }

      if (!string.IsNullOrWhiteSpace(queryParameters.EndTime))
      {
        queryParams.Add(new KeyValuePair<string, string>("endTime", queryParameters.EndTime));
      }

      if (!string.IsNullOrWhiteSpace(queryParameters.Longitude) && !string.IsNullOrWhiteSpace(queryParameters.Latitude))
      {
        queryParams.Add(new KeyValuePair<string, string>("longitude", queryParameters.Longitude));
        queryParams.Add(new KeyValuePair<string, string>("latitude", queryParameters.Latitude));
      }

      if (!string.IsNullOrWhiteSpace(queryParameters.Condition))
      {
        queryParams.Add(new KeyValuePair<string, string>("condition", queryParameters.Condition));
      }

      var queryString = string.Join("&", queryParams.Select(kvp => kvp.Key + "=" + Uri.EscapeDataString(kvp.Value)));
      url += (queryString.Length > 0 ? "/?" + queryString : string.Empty);

      var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
      if (!response.IsSuccessStatusCode)
      {
        throw new HttpRequestException($"Failed to get a successful response from the weather API. Status code: {response.StatusCode}");
      }

      return await JsonSerializer.DeserializeAsync<List<WeatherModelCelsius>>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).ConfigureAwait(false);
    }
    catch (Exception e)
    {
      throw new ConnectionFailedException($"An exception occurred while trying to get weather from the web. Details: {e.Message}", e);
    }
  }

  public async Task<List<WeatherModelCelsius>> GetWeather(string? location, string? startTime, string? endTime, string? longitude, string? latitude, string? condition)
  {
    return await GetWeather(new WeatherQueryParameters(location, startTime, endTime, longitude, latitude, condition)).ConfigureAwait(false);
  }

  #endregion
}

class WeatherQueryParameters
{
  public string Location { get; init; }
  public string StartTime { get; init; }
  public string EndTime { get; init; }
  public string Longitude { get; init; }
  public string Latitude { get; init; }
  public string Condition { get; init; }

  public WeatherQueryParameters(string? location = null, string? startTime = null, string? endTime = null, string? longitude = null, string? latitude = null, string? condition = null)
  {
    Location = location ?? string.Empty;
    StartTime = startTime ?? string.Empty;
    EndTime = endTime ?? string.Empty;
    Longitude = longitude ?? string.Empty;
    Latitude = latitude ?? string.Empty;
    Condition = condition ?? string.Empty;
  }
}