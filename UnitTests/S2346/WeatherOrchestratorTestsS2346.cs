using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using TestLibrary.S2346;
using TestLibrary.S2346.Accessor;
using TestLibrary.S2346.Models;
using WireMock.Server;
using Xunit;

namespace UnitTests.S2346;

public partial class WeatherOrchestratorTests: IDisposable
{
  private readonly WeatherOrchestrator _weatherOrchestrator;
  private WireMockServer? _wireMockServer;

  public WeatherOrchestratorTests()
  {
    _weatherOrchestrator = new WeatherOrchestrator(new WeatherFileAccessor(NullLogger<WeatherFileAccessor>.Instance), new WeatherDbAccessor(NullLogger<WeatherDbAccessor>.Instance), new WeatherApiAccessor(NullLogger<WeatherApiAccessor>.Instance), new WeatherMockAccessor(NullLogger<WeatherMockAccessor>.Instance), NullLogger<WeatherOrchestrator>.Instance);
  }

  [Fact]
  public async Task WeatherOrchestrator_AccessModeNone_ShouldFail()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessModes.Nothing);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeFalse();
    result.Exception.Should().BeAssignableTo<ArgumentException>().Which.ParamName.Should().Be("mode");
  }

  [Fact]
  public async Task WeatherOrchestrator_InvalidAccessMode_ShouldFail()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather((AccessModes) (32));

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNull().And.BeEmpty();
  }
  
  [Fact]
  public async Task WeatherOrchestrator_CombinedAccessMode_ShouldReturnCombinedList()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessModes.File | AccessModes.Mock);

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
    var result = await _weatherOrchestrator.GetWeather(AccessModes.File | AccessModes.Mock, "InvalidFile.json");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(10);
    result.Payload.Should().OnlyContain(x => x.Temperature >= -20 && x.Temperature <= 50);
  }

  public void Dispose()
  {
    _wireMockServer?.Stop();
    _wireMockServer?.Dispose();
  }
}
