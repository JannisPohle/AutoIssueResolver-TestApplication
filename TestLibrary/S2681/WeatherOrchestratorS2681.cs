using TestLibrary.S2681.Models;

namespace TestLibrary.S2681.Accessor;

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
        weatherData.Add(new WeatherModelCelsius((int) weatherModel.Temperature));
      }

      Logger.LogInformation("Found {WeatherDataCount} weather data for location {Argument}.", weatherData.Count, argument);

      return weatherData;
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error retrieving weather data from API");
      throw; // rethrow the exception to be caught by calling method
    }
  }

  #endregion
}