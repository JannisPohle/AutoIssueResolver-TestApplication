using FluentAssertions;
using TestLibrary.S4015.Models;
using Xunit;

namespace UnitTests.S4015;

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

  [Theory]
  [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
  [InlineData("asdf")]
  public async Task WeatherOrchestrator_Mock_WithArgument_ShouldReturnSameData(string argument)
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

  [Theory]
  [InlineData("")]
  [InlineData("  ")]
  [InlineData("     ")]
  [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
  [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
  public async Task WeatherOrchestrator_Mock_WithArgument_ArgumentInvalid_ShouldFail(string argument)
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Mock, argument);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeFalse();
    result.Exception.Should().NotBeNull();
    result.Exception.Should().BeAssignableTo<ArgumentException>();
  }
}
