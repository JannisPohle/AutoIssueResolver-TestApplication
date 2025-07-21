using FluentAssertions;
using TestLibrary.S2696.Models;
using Xunit;

namespace UnitTests.S2696;

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
    result.Exception.Should().BeOfType<NotImplementedException>();
  }
}
