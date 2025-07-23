using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using TestLibrary.S2629;
using TestLibrary.S2629.Accessor;
using TestLibrary.S2629.Models;
using WireMock.Server;
using Xunit;

namespace UnitTests.S2629;

public sealed partial class WeatherOrchestratorTests: IDisposable
{
  private readonly WeatherOrchestrator _weatherOrchestrator;
  private WireMockServer? _wireMockServer;
  private readonly ILogger<WeatherOrchestrator> _logger;

  public WeatherOrchestratorTests()
  {
    _logger = Substitute.For<ILogger<WeatherOrchestrator>>();

    _weatherOrchestrator = new WeatherOrchestrator(new WeatherFileAccessor(NullLogger<WeatherFileAccessor>.Instance), new WeatherDbAccessor(NullLogger<WeatherDbAccessor>.Instance), new WeatherApiAccessor(NullLogger<WeatherApiAccessor>.Instance), new WeatherMockAccessor(NullLogger<WeatherMockAccessor>.Instance), _logger);
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

  [Fact]
  public async Task WeatherOrchestrator_Success_ShouldLogAccessModeAndArgument()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Mock, "test-argument");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty();

    _logger.Received(1).Log(LogLevel.Information, Arg.Any<EventId>(),
                            Arg.Is<object>(v => v.ToString()!.Contains("Getting weather from") && v.ToString()!.Contains("Mock") && v.ToString()!.Contains("test-argument")),
                            Arg.Any<Exception>(),
                            Arg.Any<Func<object, Exception?, string>>());
  }

  public void Dispose()
  {
    _wireMockServer?.Stop();
    _wireMockServer?.Dispose();
  }
}
