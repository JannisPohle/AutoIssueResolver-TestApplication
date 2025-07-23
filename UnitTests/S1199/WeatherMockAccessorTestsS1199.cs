using FluentAssertions;
using TestLibrary.S1199.Models;
using Xunit;

namespace UnitTests.S1199;

public partial class WeatherOrchestratorTests
{
  [Fact]
  public async Task WeatherOrchestrator_Mock_NoArgument_ShouldReturnRandomData()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Mock);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(10);
    result.Payload.Should().OnlyContain(x => x.Temperature >= -20 && x.Temperature <= 50);
    result.Payload.Should().NotBeEquivalentTo((await _weatherOrchestrator.GetWeather(AccessMode.Mock)).Payload);
    result.Payload.Should().AllSatisfy(weather => weather.Unit.Should().Be("Celsius"));
  }

  [Fact]
  public async Task WeatherOrchestrator_Mock_WithArgument_ShouldReturnSameData()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Mock, "asdf");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(10);
    result.Payload.Should().OnlyContain(x => x.Temperature >= -20 && x.Temperature <= 50);
    result.Payload.Should().BeEquivalentTo((await _weatherOrchestrator.GetWeather(AccessMode.Mock, "asdf")).Payload);
    result.Payload.Should().AllSatisfy(weather => weather.Unit.Should().Be("Celsius"));
  }

  [Fact]
  public async Task WeatherOrchestrator_Mock_WithArgument_ShouldReturnSpecifiedNumberOfEntries()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Mock, "5");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(5);
    result.Payload.Should().OnlyContain(x => x.Temperature >= -20 && x.Temperature <= 50);
    result.Payload.Should().BeEquivalentTo((await _weatherOrchestrator.GetWeather(AccessMode.Mock, "5")).Payload);
    result.Payload.Should().AllSatisfy(weather => weather.Unit.Should().Be("Celsius"));
  }

  [Fact]
  public async Task WeatherOrchestrator_Mock_WithArgument_NoDataFound_ShouldThrowException()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Mock, "0");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeFalse();
    result.Exception.Should().BeOfType<DataNotFoundException>();
  }
}
