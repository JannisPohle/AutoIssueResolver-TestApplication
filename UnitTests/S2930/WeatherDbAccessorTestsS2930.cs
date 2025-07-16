using FluentAssertions;
using TestLibrary.S2930.Models;
using Xunit;

namespace UnitTests.S2930;

public partial class WeatherOrchestratorTests
{
  [Fact]
  public async Task WeatherOrchestrator_DbAccessor_Dummy()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Database);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeFalse();
    result.Exception.Should().BeOfType<NotImplementedException>();
  }
}
