using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Interfaces;
using TestLibrary.S2372.Models;
using TestLibrary.S2372.Models.External;
using WireMock.FluentAssertions;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace UnitTests.S2372;

public partial class WeatherOrchestratorTests
{
  [Fact]
  public async Task WeatherOrchestrator_ApiAccessor_GetDefaultValues()
  {
    // Arrange
    SetupWireMockServer();

    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Web);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(3);
    result.Payload.Should().Satisfy(w => w.Temperature == 20, w => w.Temperature == 17, w => w.Temperature == 11);

    _wireMockServer!.Should().HaveReceivedACall().AtUrl("http://localhost:31246/v1/api/weather").And.UsingGet();
  }

  [Fact]
  public async Task WeatherOrchestrator_ApiAccessor_WithParam_EmptyList()
  {
    // Arrange
    SetupWireMockServer();

    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Web, "Stuttgart");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNull().And.BeEmpty();

    _wireMockServer!.Should().HaveReceivedACall().AtUrl("http://localhost:31246/v1/api/weather?location=Stuttgart").And.UsingGet();
  }

  [Fact]
  public async Task WeatherOrchestrator_ApiAccessor_WithParam_SingleValue()
  {
    // Arrange
    SetupWireMockServer();

    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Web, "Berlin");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(1);
    result.Payload.Should().Satisfy(w => w.Temperature == 11);

    _wireMockServer!.Should().HaveReceivedACall().AtUrl("http://localhost:31246/v1/api/weather?location=Berlin").And.UsingGet();
  }

  [Fact]
  public async Task WeatherOrchestrator_ApiAccessor_WithParam_InvalidValue()
  {
    // Arrange
    SetupWireMockServer();

    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Web, "Bielefeld");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeFalse();
    result.Exception.Should().BeAssignableTo<ConnectionFailedException>();

    _wireMockServer!.Should().HaveReceivedACall().AtUrl("http://localhost:31246/v1/api/weather?location=Bielefeld").And.UsingGet();
  }


  private void SetupWireMockServer()
  {
    _wireMockServer = WireMockServer.Start(31246);
    _wireMockServer?.Given(Request.Create().WithPath("/v1/api/weather").WithParam("location", MatchBehaviour.RejectOnMatch).UsingGet())
                   .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(new List<WeatherApiModel>
                   {
                     new() { Location = "Munich", Temperature = 20, Condition = "Sunny" },
                     new() { Location = "Hamburg", Temperature = 17.2, Condition = "Cloudy" },
                     new() { Location = "Berlin", Temperature = 11.9, Condition = "Rainy" },
                   }));

    _wireMockServer?.Given(Request.Create().WithPath("/v1/api/weather").WithParam("location").UsingGet())
                   .RespondWith(Response.Create().WithStatusCode(404));

    _wireMockServer?.Given(Request.Create().WithPath("/v1/api/weather").WithParam("location", MatchBehaviour.AcceptOnMatch, "Stuttgart").UsingGet())
                   .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(new List<WeatherApiModel>()));

    _wireMockServer?.Given(Request.Create().WithPath("/v1/api/weather").WithParam("location", MatchBehaviour.AcceptOnMatch, "Berlin").UsingGet())
                   .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(new List<WeatherApiModel>
                   {
                     new() { Location = "Berlin", Temperature = 11.9, Condition = "Rainy" },
                   }));

  }
}
