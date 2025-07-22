using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using TestLibrary.S1479;
using TestLibrary.S1479.Accessor;
using TestLibrary.S1479.Models;
using WireMock.Server;
using Xunit;

namespace UnitTests.S1479;

public sealed partial class WeatherOrchestratorTests: IDisposable
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
    var result = await _weatherOrchestrator.GetWeather(AccessMode.None.ToString());

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeFalse();
    result.Exception.Should().BeAssignableTo<ArgumentException>().Which.ParamName.Should().Be("mode");
  }

  [Fact]
  public async Task WeatherOrchestrator_InvalidAccessMode_ShouldFail()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather("FooBar");


    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeFalse();
    result.Exception.Should().BeAssignableTo<ArgumentException>().Which.ParamName.Should().Be("mode");
  }

  public void Dispose()
  {
    _wireMockServer?.Stop();
    _wireMockServer?.Dispose();
  }
}
