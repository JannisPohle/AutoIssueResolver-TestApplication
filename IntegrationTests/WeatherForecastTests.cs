using System.Net;
using System.Text.Json;
using FluentAssertions;
using TestLibrary.Template.Models;
using Xunit;

namespace IntegrationTests;

public class WeatherForecastTests
{

  [Fact]
  public async Task GetWeatherForecast()
  {
    // Arrange
    await using var factory = new WebApplicationFactory();
    var client = factory.CreateClient();

    // Act
    var response = await client.GetAsync($"/WeatherForecast/Template");

    // Assert
    response.EnsureSuccessStatusCode();
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var content = await JsonSerializer.DeserializeAsync<List<WeatherModelCelsius>>(await response.Content.ReadAsStreamAsync());
    content.Should().NotBeNull();
    content!.Count.Should().Be(5);
    content[0].Date.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
  }
}