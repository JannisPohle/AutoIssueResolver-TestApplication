using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using TestLibrary.S2696;
using TestLibrary.S2696.Accessor;
using TestLibrary.S2696.Models;
using Xunit;

namespace UnitTests.S2696;

[TestCaseOrderer("UnitTests.AlphabeticalOrderer", "UnitTests")]
public partial class WeatherOrchestratorTests
{
  [Fact]
  public async Task WeatherOrchestrator_1_Mock_NoArgument_ShouldReturnDeterministicRandomData()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Mock);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(1); // number of entries should default to the number of calls to the accessor (global, across test cases)
    result.Payload.First().Should().Satisfy<WeatherModelCelsius>(x => x.Temperature.Should().Be(-4));
    result.Payload.Should().AllSatisfy(weather => weather.Unit.Should().Be("Celsius"));

    var secondResult = await _weatherOrchestrator.GetWeather(AccessMode.Mock);
    secondResult.Payload.Should().NotBeNullOrEmpty().And.HaveCount(2);
    secondResult.Payload.Should().AllSatisfy(weather => weather.Unit.Should().Be("Celsius"));
    secondResult.Payload.Should().Satisfy(x => x.Temperature == 30, x => x.Temperature == 6);

    result.Payload.Should().NotBeEquivalentTo(secondResult.Payload);
  }

  [Fact]
  public async Task WeatherOrchestrator_2_Mock_NoArgument_ShouldReturnDeterministicData_WithMultipleInstances()
  {
    var secondOrchestrator = new WeatherOrchestrator(new WeatherFileAccessor(NullLogger<WeatherFileAccessor>.Instance), new WeatherDbAccessor(NullLogger<WeatherDbAccessor>.Instance), new WeatherApiAccessor(NullLogger<WeatherApiAccessor>.Instance), new WeatherMockAccessor(NullLogger<WeatherMockAccessor>.Instance), NullLogger<WeatherOrchestrator>.Instance);
    // Act
    var result = await secondOrchestrator.GetWeather(AccessMode.Mock);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(3); // number of entries should default to the number of calls to the accessor (global, across test cases)
    result.Payload.Should().Satisfy(x => x.Temperature == -1, x => x.Temperature == 25, x => x.Temperature == 36);
    result.Payload.Should().AllSatisfy(weather => weather.Unit.Should().Be("Celsius"));

    var secondResult = await _weatherOrchestrator.GetWeather(AccessMode.Mock);
    secondResult.Payload.Should().NotBeNullOrEmpty().And.HaveCount(4);
    secondResult.Payload.Should().AllSatisfy(weather => weather.Unit.Should().Be("Celsius"));
    secondResult.Payload.Should().Satisfy(x => x.Temperature == 33, x => x.Temperature == 44, x => x.Temperature == 16, x => x.Temperature == 6);

    result.Payload.Should().NotBeEquivalentTo(secondResult.Payload);
  }

  [Fact]
  public async Task WeatherOrchestrator_3_Mock_WithArgument_ShouldReturnSameData()
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
