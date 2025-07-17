using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using TestLibrary.S2306;
using TestLibrary.S2306.Accessor;
using TestLibrary.S2306.Models;
using Xunit;

namespace UnitTests.S2306;

public partial class WeatherOrchestratorTests
{
  private readonly WeatherOrchestrator _weatherOrchestrator;

  public WeatherOrchestratorTests()
  {
    _weatherOrchestrator = new WeatherOrchestrator(new WeatherFileAccessor(NullLogger<WeatherFileAccessor>.Instance), new WeatherDbAccessor(NullLogger<WeatherDbAccessor>.Instance), new WeatherApiAccessor(NullLogger<WeatherApiAccessor>.Instance), new WeatherMockAccessor(NullLogger<WeatherMockAccessor>.Instance), NullLogger<WeatherOrchestrator>.Instance);
  }

  [Fact]
  public async Task WeatherOrchestrator_AccessModeNone_ShouldFail()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.None);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeFalse();
    result.Exception.Should().BeAssignableTo<ArgumentException>().Which.ParamName.Should().Be("mode");
  }

  [Fact]
  public async Task WeatherOrchestrator_InvalidAccessMode_ShouldFail()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather((AccessMode) (-12));

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeFalse();
    result.Exception.Should().BeAssignableTo<ArgumentException>().Which.ParamName.Should().Be("mode");
  }
}
