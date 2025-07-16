using FluentAssertions;
using TestLibrary.Template.Models;
using Xunit;

namespace UnitTests.Template;

public partial class WeatherOrchestratorTests
{
  [Fact]
  public async Task WeatherOrchestrator_File_NoArgument_ShouldUseFallbackFile()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.File);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(5);
    result.Payload.Should().OnlyContain(x => x.Temperature >= -5 && x.Temperature <= 20 && x.Temperature != 0);
  }

  [Fact]
  public async Task WeatherOrchestrator_File_WithArgument_ShouldUseSpecifiedFile()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.File, "TestFiles/WeatherForecast_10Entries.json");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(10);
    result.Payload.Should().OnlyContain(x => x.Temperature >= -12 && x.Temperature <= 25 && x.Temperature != 0);
  }

  [Fact]
  public async Task WeatherOrchestrator_File_WithArgument_NoDataFound_ShouldReturnNoData()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.File, "TestFiles/EmptyWeatherForecast.json");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNull().And.HaveCount(0);
  }

  [Fact]
  public async Task WeatherOrchestrator_File_WithArgument_InvalidData_ShouldFail()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.File, "TestFiles/InvalidWeatherForecast.json");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeFalse();
    result.Exception.Should().NotBeNull();
  }

  [Fact]
  public async Task WeatherOrchestrator_File_WithArgument_FileNotFound_ShouldFail()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.File, $"TestFiles/{Guid.NewGuid()}.json");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeFalse();
    result.Exception.Should().NotBeNull();
  }
}