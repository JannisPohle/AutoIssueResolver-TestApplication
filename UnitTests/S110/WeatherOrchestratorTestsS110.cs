using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using TestLibrary.S110;
using TestLibrary.S110.Accessor;
using TestLibrary.S110.Models;
using WireMock.Server;
using Xunit;

namespace UnitTests.S110;

public partial class WeatherOrchestratorTests: IDisposable
{
  private readonly WeatherOrchestrator _weatherOrchestrator;
  private WireMockServer? _wireMockServer;
  private ILogger<WeatherDbAccessor> _dbAccessorLogger;
  private ILogger<WeatherFileAccessor> _fileAccessorLogger;
  private ILogger<WeatherApiAccessor> _apiAccessorLogger;
  private ILogger<WeatherMockAccessor> _mockAccessorLogger;

  public WeatherOrchestratorTests()
  {
    _dbAccessorLogger = Substitute.For<ILogger<WeatherDbAccessor>>();
    _fileAccessorLogger = Substitute.For<ILogger<WeatherFileAccessor>>();
    _apiAccessorLogger = Substitute.For<ILogger<WeatherApiAccessor>>();
    _mockAccessorLogger = Substitute.For<ILogger<WeatherMockAccessor>>();
    _weatherOrchestrator = new WeatherOrchestrator(new WeatherFileAccessor(_fileAccessorLogger), new WeatherDbAccessor(_dbAccessorLogger), new WeatherApiAccessor(_apiAccessorLogger), new WeatherMockAccessor(_mockAccessorLogger), NullLogger<WeatherOrchestrator>.Instance);
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

  public void Dispose()
  {
    _wireMockServer?.Stop();
    _wireMockServer?.Dispose();
  }
}
