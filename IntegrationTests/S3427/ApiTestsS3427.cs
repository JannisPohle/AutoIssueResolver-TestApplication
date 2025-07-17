using System.Net;
using System.Text.Json;
using FluentAssertions;
using TestLibrary.S3427.Models;
using Xunit;

namespace IntegrationTests.S3427;

public class ApiTestsS3427
{

  [Fact]
  public async Task GetWithDefaultValues_ShouldReturnSuccessfully()
  {
    // Arrange
    await using var factory = new WebApplicationFactory();
    var client = factory.CreateClient();

    // Act
    var response = await client.GetAsync($"/S3427");

    // Assert
    response.EnsureSuccessStatusCode();
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var content = await JsonSerializer.DeserializeAsync<List<WeatherModelCelsius>>(await response.Content.ReadAsStreamAsync());
    content.Should().NotBeNull();
    content!.Count.Should().Be(5);
    content[0].Date.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
  }

  [Fact]
  public async Task GetInvalidArgument_DoNotThrow_ShouldReturnEmptyList()
  {
    // Arrange
    await using var factory = new WebApplicationFactory();
    var client = factory.CreateClient();

    // Act
    var response = await client.GetAsync($"/S3427?argument=foo.json&throwOnError=false");

    // Assert
    response.EnsureSuccessStatusCode();
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var content = await JsonSerializer.DeserializeAsync<List<WeatherModelCelsius>>(await response.Content.ReadAsStreamAsync());
    content.Should().NotBeNull();
    content!.Count.Should().Be(0);
  }

  [Fact]
  public async Task GetInvalidArgument_Throw_ShouldReturnError()
  {
    // Arrange
    await using var factory = new WebApplicationFactory();
    var client = factory.CreateClient();

    // Act
    var response = await client.GetAsync($"/S3427?argument=foo.json&throwOnError=true");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }
}