using FluentAssertions;
using TestLibrary.S6962.Models;
using Xunit;

namespace UnitTests.S6962;

public partial class WeatherOrchestratorTests
{
  [Fact]
  public async Task WeatherOrchestrator_ApiAccessor_Dummy()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Web);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeFalse();
    //TODO setup test
  }
}
