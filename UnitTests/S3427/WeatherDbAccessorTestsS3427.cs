using FluentAssertions;
using TestLibrary.S3427.Models;
using Xunit;

namespace UnitTests.S3427;

public partial class WeatherOrchestratorTests
{
  [Fact]
  public async Task WeatherOrchestrator_DbAccessor_Dummy()
  {
    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Database, null);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeFalse();
    result.Exception.Should().BeOfType<NotImplementedException>();
  }
}
