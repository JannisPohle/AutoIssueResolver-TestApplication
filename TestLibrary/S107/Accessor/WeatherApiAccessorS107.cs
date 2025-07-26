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

      var response = _httpClient.GetFromJsonAsAsyncEnumerable<Models.External.WeatherApiModel>(url + queryUrl);

      if (response is null)
      {
        Logger.LogWarning("No weather data found for with arguments: {Query}", queryUrl);

        throw new DataNotFoundException($