using System.Globalization;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using TestLibrary.S6561;
using TestLibrary.S6561.Accessor;
using TestLibrary.S6561.Models;
using WireMock.Server;
using Xunit;

namespace UnitTests.S6561;

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
  public async Task WeatherOrchestrator_InvalidAccessMode_ShouldFail_And_LogExecutionTime()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather((AccessMode) (-12));

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeFalse();
    result.Exception.Should().BeAssignableTo<ArgumentException>().Which.ParamName.Should().Be("mode");

    _logger.Received(1).Log(LogLevel.Information, Arg.Any<EventId>(),
                            Arg.Is<object>(v => IsValidExecutionTime(v.ToString()!)),
                            Arg.Any<Exception>(),
                            Arg.Any<Func<object, Exception?, string>>());
  }

  [Fact]
  public async Task WeatherOrchestrator_Success_ShouldLogExecutionTime()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Mock);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty();

    _logger.Received(1).Log(LogLevel.Information, Arg.Any<EventId>(),
                            Arg.Is<object>(v => IsValidExecutionTime(v.ToString()!)),
                            Arg.Any<Exception>(),
                            Arg.Any<Func<object, Exception?, string>>());
  }

  public void Dispose()
  {
    _wireMockServer?.Stop();
    _wireMockServer?.Dispose();
  }

  private static bool IsValidExecutionTime(string logMessage)
  {
    var match = Regex.Match(logMessage, @"Finished getting weather data after ([0-9\.:]+) ms");
    if (!match.Success)
    {
      return false;
    }

    var time = TimeSpan.Zero;

    if (!double.TryParse(match.Groups[1].Value, out var elapsedMilliseconds) && !TimeSpan.TryParse(match.Groups[1].Value, CultureInfo.InvariantCulture, out time))
    {
      return false;
    }

    // Assuming a reasonable execution time limit
    return elapsedMilliseconds is > 0 and < 10000 || (time > TimeSpan.Zero && time < TimeSpan.FromSeconds(10));
  }
}
