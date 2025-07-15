using FluentAssertions;
using TestLibrary.S1155;
using Xunit;

namespace UnitTests;

public class S1155Tests
{

  [Fact]
  public async Task LoadWeatherForecasts_ShouldLoadEntriesFromFile()
  {
    // Arrange
    var accessor = new WeatherAccessor("TestFiles/WeatherForecast.json");

    // Act
    var result = await accessor.LoadWeatherForecasts();

    // Assert
    result.Should().NotBeNullOrEmpty().And.HaveCount(5);
  }

  [Fact]
  public async Task LoadWeatherForecasts_EmptyFile_ShouldThrowException()
  {
    // Arrange
    var accessor = new WeatherAccessor("TestFiles/EmptyWeatherForecast.json");

    // Act
    var action = async () => await accessor.LoadWeatherForecasts();

    // Assert
    (await action.Should().ThrowAsync<InvalidOperationException>()).Which.Message.Should().Be("The file is empty.");
  }

}