using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using TestLibrary.S3458;
using TestLibrary.S3458.Accessor;
using TestLibrary.S3458.Models;
using WireMock.Server;
using Xunit;

namespace UnitTests.S3458;

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

  [Theory]
  [InlineData(AccessMode.None)]
  [InlineData((AccessMode) (-12))]
  public async Task WeatherOrchestrator_AccessModeNone_ShouldReturnEmptyList_And_LogWarning(AccessMode mode)
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(mode);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNull().And.BeEmpty();

    _logger.Received(1).Log(LogLevel.Warning, Arg.Any<EventId>(),
                            Arg.Is<object>(v => v.ToString()!.Contains("No valid access mode provided")),
                            Arg.Any<Exception>(),
                            Arg.Any<Func<object, Exception?, string>>());
  }

  public void Dispose()
  {
    _wireMockServer?.Stop();
    _wireMockServer?.Dispose();
  }
}
