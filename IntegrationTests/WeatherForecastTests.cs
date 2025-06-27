using Xunit;

namespace IntegrationTests;

public class WeatherForecastTests
{

  [Fact]
  public async Task GetWeatherForecast_ReturnsSuccess()
  {
    // Arrange
    await using var factory = new WebApplicationFactory();
    var client = factory.CreateClient();

    // Act
    var response = await client.GetAsync("/WeatherForecast");

    // Assert
    response.EnsureSuccessStatusCode();
  }
}