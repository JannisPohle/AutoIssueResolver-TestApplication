using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using TestLibrary.S2629.Accessor;
using TestLibrary.S2629.Models;
using Xunit;

namespace UnitTests.S2629;

public partial class WeatherOrchestratorTests
{
  [Fact]
  public async Task WeatherOrchestrator_DbAccessor_ShouldReturnWeatherData()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Database);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(10);
    result.Payload.Should().OnlyContain(x => x.Temperature >= 18 && x.Temperature <= 23 && x.Temperature != 0);
    result.Payload.Should().AllSatisfy(weather => weather.Unit.Should().Be("Celsius"));
  }

  [Fact]
  public async Task DbAccessor_ShouldThrowException_WhenConnectionWasNotOpened()
  {
    // Arrange
    var dbAccessor = new WeatherDbAccessor(NullLogger<WeatherDbAccessor>.Instance);
    var action = async () => await dbAccessor.GetWeather(null);

    // Act & Assert
    (await action.Should().ThrowAsync<InvalidOperationException>()).And.Message.Should().Contain("Database connection is not open.");
  }

  [Fact]
  public async Task WeatherOrchestrator_DbAccessor_WithArgument_InvalidConnectionString_ShouldFail()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Database, "Database/InvalidDBFile.db");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeFalse();
    result.Exception.Should().NotBeNull();
    result.Exception.Should().BeAssignableTo<ConnectionFailedException>();
  }
}
