using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using TestLibrary.S3265;
using TestLibrary.S3265.Accessor;
using TestLibrary.S3265.Models;
using Xunit;

namespace UnitTests.S3265;

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
  public async Task WeatherOrchestrator_InvalidAccessMode_ShouldReturnEmptyList()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather((AccessMode) (32));

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNull().And.BeEmpty();
  }

  [Fact]
  public async Task WeatherOrchestrator_CombinedAccessMode_ShouldReturnCombinedList()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.File | AccessMode.Mock);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(15);
    result.Payload.Should().OnlyContain(x => x.Temperature >= -20 && x.Temperature <= 50);
  }

  [Fact]
  public async Task WeatherOrchestrator_CombinedAccessMode_ShouldReturnCombinedList_EvenIfOneSourceFails()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.File | AccessMode.Mock, "InvalidFile.json");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(10);
    result.Payload.Should().OnlyContain(x => x.Temperature >= -20 && x.Temperature <= 50);
  }
}
