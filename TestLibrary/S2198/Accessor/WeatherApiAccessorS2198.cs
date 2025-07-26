using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TestLibrary.S2198.Models;

namespace TestLibrary.S2198.Accessor;

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

      var response = _httpClient.GetFromJsonAsAsyncEnumerable<Models.External.WeatherApiModel>(url);

      if (response is null)
      {
        Logger.LogWarning("No weather data found for argument: {Argument}", argument);

        throw new DataNotFoundException($"No weather data found for argument: {argument}.");
      }

      var weatherData = new List<WeatherModelCelsius>();

      await foreach (var weatherModel in response)
      {
        if (weatherModel.Temperature < -40)
        {
          throw new ValidationException("Temperature value is too low.");
        }

        weatherData.Add(new WeatherModelCelsius((int) weatherModel.Temperature));

        Logger.LogDebug("Weather data for location {Location}: {Temperature}°C, Condition: {Condition}", weatherModel.Location, weatherModel.Temperature, weatherModel.Condition);
      }

      Logger.LogInformation("Found {WeatherDataCount} weather data for location {Argument}.", weatherData.Count, argument);

      return weatherData;
    }
    catch (Exception e) when (e is not ValidationException)
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
