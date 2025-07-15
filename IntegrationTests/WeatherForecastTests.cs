using System.Net;
using System.Text.Json;
using FluentAssertions;
using WebApplication;
using Xunit;

namespace IntegrationTests;

public class WeatherForecastTests
{

  [Theory]
  [InlineData("S2930")]
  [InlineData("S2931")]
  public async Task GetWeatherForecast_ReturnsSuccess(string path)
  {
    // Arrange
    await using var factory = new WebApplicationFactory();
    var client = factory.CreateClient();

    // Act
    var response = await client.GetAsync($"/WeatherForecast/{path}");

    // Assert
    response.EnsureSuccessStatusCode();
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var content = await JsonSerializer.DeserializeAsync<List<WeatherForecast>>(await response.Content.ReadAsStreamAsync());
    content.Should().NotBeNull();
    content!.Count.Should().Be(5);
  }
}