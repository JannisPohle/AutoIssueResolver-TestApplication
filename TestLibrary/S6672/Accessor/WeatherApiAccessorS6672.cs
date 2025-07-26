using Microsoft.Extensions.Logging;
using TestLibrary.S6672.Models;

namespace TestLibrary.S6672.Accessor;

public sealed class WeatherApiAccessor : WeatherAccessorBase, IDisposable
{
  #region Members

  private readonly HttpClient _httpClient = new();

  #endregion

  #region Constructors

  public WeatherApiAccessor(ILogger<WeatherApiAccessor> logger)
    : base(logger) { }

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
        Logger.LogWarning("No weather data found for argument: {{Argument}}", argument);

        throw new DataNotFoundException($"No weather data found for argument: {{argument}}");
      }

      // Assuming some processing of response happens here to create WeatherModelCelsius list
      return await ProcessResponse(response);
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error retrieving weather data from API.");
      throw;
    }
  }

  private async Task<List<WeatherModelCelsius>> ProcessResponse(IAsyncEnumerable<Models.External.WeatherApiModel> response)
  {
    var resultList = new List<WeatherModelCelsius>();
    await foreach (var item in response)
    {
      // Convert each WeatherApiModel to WeatherModelCelsius
      resultList.Add(new WeatherModelCelsius((int)item.Temperature));
    }
    return resultList;
  }

  #endregion
}