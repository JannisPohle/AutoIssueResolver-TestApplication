using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S6962.Models;

namespace TestLibrary.S6962.Accessor;

public sealed class WeatherApiAccessor: WeatherAccessorBase, IDisposable
{
  private readonly HttpClient _httpClient;
  private bool _disposed;

  public WeatherApiAccessor(ILogger<WeatherApiAccessor> logger)
    : base(logger)
  {
    _httpClient = new HttpClient();
  }

  public override async Task<List<WeatherModelCelsius>> GetWeather(string? argument)
  {
    try
    {
      var result = await _httpClient.GetFromJsonAsync<List<WeatherModelCelsius>>($"http://localhost:31246/api/weather?location={argument}");

      if (result == null || result.Count == 0)
      {
        Logger.LogWarning("No data found from API endpoint for location: {Argument}", argument);
        throw new DataNotFoundException($"No data found from API endpoint for location: {argument}.");
      }

      return result;
    }
    catch (Exception e)
    {
      Logger.LogWarning(e, "An error occured while getting weather data  for location: {Argument}.", argument);
      //TODO throw connection exception
      throw;
    }
  }

  private void Dispose(bool disposing)
  {
    if (_disposed)
    {
      return;
    }

    if (disposing)
    {
      // Dispose managed resources
      _httpClient?.Dispose();
    }

    // Dispose unmanaged resources
    _disposed = true;
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
}
