using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using TestLibrary.S107.Accessor;
using TestLibrary.S107.Models;
using TestLibrary.S107.Models.External;
using WireMock.FluentAssertions;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace UnitTests.S107;

public partial class WeatherOrchestratorTests
{
  private void SetupWireMockServer()
  {
    _wireMockServer = WireMockServer.Start(31246);

    _wireMockServer?.Given(Request.Create().WithPath("/v1/api/weather").WithParam("location", MatchBehaviour.RejectOnMatch).UsingGet())
                   .RespondWith(Response.Create()
                                        .WithStatusCode(200)
                                        .WithBodyAsJson(new List<WeatherApiModel>
                                        {
                                          new()
                                          {
                                            Location = "Munich",
                                            Temperature = 20,
                                            Condition = "Sunny",
                                          },
                                          new()
                                          {
                                            Location = "Hamburg",
                                            Temperature = 17.2,
                                            Condition = "Cloudy",
                                          },
                                          new()
                                          {
                                            Location = "Berlin",
                                            Temperature = 11.9,
                                            Condition = "Rainy",
                                          },
                                        }));

    _wireMockServer?.Given(Request.Create().WithPath("/v1/api/weather").WithParam("location").UsingGet())
                   .RespondWith(Response.Create().WithStatusCode(404));

    _wireMockServer?.Given(Request.Create().WithPath("/v1/api/weather").WithParam("location", MatchBehaviour.AcceptOnMatch, "Stuttgart").UsingGet())
                   .RespondWith(Response.Create().WithStatusCode(200).WithBodyAsJson(new List<WeatherApiModel>()));

    _wireMockServer?.Given(Request.Create().WithPath("/v1/api/weather").WithParam("location", MatchBehaviour.AcceptOnMatch, "Berlin").UsingGet())
                   .RespondWith(Response.Create()
                                        .WithStatusCode(200)
                                        .WithBodyAsJson(new List<WeatherApiModel>
                                        {
                                          new()
                                          {
                                            Location = "Berlin",
                                            Temperature = 11.9,
                                            Condition = "Rainy",
                                          },
                                        }));
  }

  [Fact]
  public async Task GetWeather_WithAllParameters_SendsCorrectQuery()
  {
    // Arrange
    SetupWireMockServer();

    _wireMockServer!.Given(Request.Create()
                                  .WithPath("/v1/api/weather")
                                  .WithParam("location", "Rome")
                                  .WithParam("startTime", "2024-06-01T08:00:00")
                                  .WithParam("endTime", "2024-06-01T20:00:00")
                                  .WithParam("longitude", "12.5")
                                  .WithParam("latitude", "41.9")
                                  .WithParam("unit", "celsius")
                                  .UsingGet())
                    .RespondWith(Response.Create()
                                         .WithStatusCode(200)
                                         .WithBodyAsJson(new List<WeatherApiModel>
                                         {
                                           new()
                                           {
                                             Location = "Rome",
                                             Temperature = 30,
                                             Condition = "Sunny",
                                           },
                                         }));

    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Web, "Rome;2024-06-01T08:00:00;2024-06-01T20:00:00;12.5;41.9;celsius");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(1);
    result.Payload.Should().Satisfy(w => w.Temperature == 30);

    _wireMockServer.Should().HaveReceivedACall().AtUrl("http://localhost:31246/v1/api/weather?location=Rome&startTime=2024-06-01T08:00:00&endTime=2024-06-01T20:00:00&longitude=12.5&latitude=41.9&unit=celsius").And.UsingGet();
  }

  [Fact]
  public async Task GetWeather_WithEmptyStringParameters_IgnoresThem()
  {
    // Arrange
    SetupWireMockServer();

    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Web, ";;;;;");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(3);
    result.Payload.Should().Satisfy(w => w.Temperature == 20, w => w.Temperature == 17, w => w.Temperature == 11);

    _wireMockServer!.Should().HaveReceivedACall().AtUrl("http://localhost:31246/v1/api/weather").And.UsingGet();
  }

  [Fact]
  public async Task GetWeather_WithLatitudeOnly_ReturnsExpected()
  {
    // Arrange
    SetupWireMockServer();

    _wireMockServer!.Given(Request.Create()
                                  .WithPath("/v1/api/weather")
                                  .WithParam("latitude", "47.0")
                                  .UsingGet())
                    .RespondWith(Response.Create()
                                         .WithStatusCode(200)
                                         .WithBodyAsJson(new List<WeatherApiModel>
                                         {
                                           new()
                                           {
                                             Location = "Lat",
                                             Temperature = 16,
                                             Condition = "Rain",
                                           },
                                         }));

    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Web, ";;;;47.0");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(1);
    result.Payload.Should().Satisfy(w => w.Temperature == 16);

    _wireMockServer.Should().HaveReceivedACall().AtUrl("http://localhost:31246/v1/api/weather?latitude=47.0").And.UsingGet();
  }

  [Fact]
  public async Task GetWeather_WithLongitudeLatitude_SendsCorrectQuery()
  {
    // Arrange
    SetupWireMockServer();

    _wireMockServer!.Given(Request.Create()
                                  .WithPath("/v1/api/weather")
                                  .WithParam("longitude", "10.0")
                                  .WithParam("latitude", "50.0")
                                  .UsingGet())
                    .RespondWith(Response.Create()
                                         .WithStatusCode(200)
                                         .WithBodyAsJson(new List<WeatherApiModel>
                                         {
                                           new()
                                           {
                                             Location = "Geo",
                                             Temperature = 22,
                                             Condition = "Clear",
                                           },
                                         }));

    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Web, ";;;10.0;50.0");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(1);
    result.Payload.Should().Satisfy(w => w.Temperature == 22);

    _wireMockServer.Should().HaveReceivedACall().AtUrl("http://localhost:31246/v1/api/weather?longitude=10.0&latitude=50.0").And.UsingGet();
  }

  [Fact]
  public async Task GetWeather_WithLongitudeOnly_ReturnsExpected()
  {
    // Arrange
    SetupWireMockServer();

    _wireMockServer!.Given(Request.Create()
                                  .WithPath("/v1/api/weather")
                                  .WithParam("longitude", "8.5")
                                  .UsingGet())
                    .RespondWith(Response.Create()
                                         .WithStatusCode(200)
                                         .WithBodyAsJson(new List<WeatherApiModel>
                                         {
                                           new()
                                           {
                                             Location = "Long",
                                             Temperature = 21,
                                             Condition = "Windy",
                                           },
                                         }));

    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Web, ";;;8.5");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(1);
    result.Payload.Should().Satisfy(w => w.Temperature == 21);

    _wireMockServer.Should().HaveReceivedACall().AtUrl("http://localhost:31246/v1/api/weather?longitude=8.5").And.UsingGet();
  }

  [Fact]
  public async Task GetWeather_WithOnlyEndTime_ReturnsExpected()
  {
    // Arrange
    SetupWireMockServer();

    _wireMockServer!.Given(Request.Create()
                                  .WithPath("/v1/api/weather")
                                  .WithParam("endTime", "2024-06-01T18:00:00")
                                  .UsingGet())
                    .RespondWith(Response.Create()
                                         .WithStatusCode(200)
                                         .WithBodyAsJson(new List<WeatherApiModel>
                                         {
                                           new()
                                           {
                                             Location = "Late",
                                             Temperature = 19,
                                             Condition = "Clear",
                                           },
                                         }));

    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Web, ";;2024-06-01T18:00:00");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(1);
    result.Payload.Should().Satisfy(w => w.Temperature == 19);

    _wireMockServer.Should().HaveReceivedACall().AtUrl("http://localhost:31246/v1/api/weather?endTime=2024-06-01T18:00:00").And.UsingGet();
  }

  [Fact]
  public async Task GetWeather_WithOnlyStartTime_ReturnsExpected()
  {
    // Arrange
    SetupWireMockServer();

    _wireMockServer!.Given(Request.Create()
                                  .WithPath("/v1/api/weather")
                                  .WithParam("startTime", "2024-06-01T06:00:00")
                                  .UsingGet())
                    .RespondWith(Response.Create()
                                         .WithStatusCode(200)
                                         .WithBodyAsJson(new List<WeatherApiModel>
                                         {
                                           new()
                                           {
                                             Location = "Early",
                                             Temperature = 12,
                                             Condition = "Foggy",
                                           },
                                         }));

    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Web, ";2024-06-01T06:00:00");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(1);
    result.Payload.Should().Satisfy(w => w.Temperature == 12);

    _wireMockServer.Should().HaveReceivedACall().AtUrl("http://localhost:31246/v1/api/weather?startTime=2024-06-01T06:00:00").And.UsingGet();
  }

  [Fact]
  public async Task GetWeather_WithStartTimeAndEndTime_SendsCorrectQuery()
  {
    // Arrange
    SetupWireMockServer();

    _wireMockServer!.Given(Request.Create()
                                  .WithPath("/v1/api/weather")
                                  .WithParam("startTime", "2024-06-01T00:00:00")
                                  .WithParam("endTime", "2024-06-01T12:00:00")
                                  .UsingGet())
                    .RespondWith(Response.Create()
                                         .WithStatusCode(200)
                                         .WithBodyAsJson(new List<WeatherApiModel>
                                         {
                                           new()
                                           {
                                             Location = "Munich",
                                             Temperature = 18,
                                             Condition = "Cloudy",
                                           },
                                         }));

    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Web, ";2024-06-01T00:00:00;2024-06-01T12:00:00");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(1);
    result.Payload.Should().Satisfy(w => w.Temperature == 18);

    _wireMockServer.Should().HaveReceivedACall().AtUrl("http://localhost:31246/v1/api/weather?startTime=2024-06-01T00:00:00&endTime=2024-06-01T12:00:00").And.UsingGet();
  }

  [Fact]
  public async Task GetWeather_WithUnitParameter_SendsCorrectQuery()
  {
    // Arrange
    SetupWireMockServer();

    _wireMockServer!.Given(Request.Create()
                                  .WithPath("/v1/api/weather")
                                  .WithParam("unit", "fahrenheit")
                                  .UsingGet())
                    .RespondWith(Response.Create()
                                         .WithStatusCode(200)
                                         .WithBodyAsJson(new List<WeatherApiModel>
                                         {
                                           new()
                                           {
                                             Location = "NYC",
                                             Temperature = 77,
                                             Condition = "Hot",
                                           },
                                         }));

    // Act
    var result = await _weatherOrchestrator.GetWeather(AccessMode.Web, ";;;;;fahrenheit");

    // Assert
    result.Should().NotBeNull();
    result.IsSuccess.Should().BeTrue();
    result.Payload.Should().NotBeNullOrEmpty().And.HaveCount(1);
    result.Payload.Should().Satisfy(w => w.Temperature == 77);

    _wireMockServer.Should().HaveReceivedACall().AtUrl("http://localhost:31246/v1/api/weather?unit=fahrenheit").And.UsingGet();
  }

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
  public async Task GetWeather_OnlySingleParameter_SingleValue()
  {
    // Arrange
    SetupWireMockServer();
    var weatherApiAccessor = new WeatherApiAccessor(NullLogger<WeatherApiAccessor>.Instance);

    // Act
    var result = await weatherApiAccessor.GetWeather("Berlin");

    // Assert
    result.Should().NotBeNullOrEmpty().And.HaveCount(1);
    result.Should().Satisfy(w => w.Temperature == 11);

    _wireMockServer!.Should().HaveReceivedACall().AtUrl("http://localhost:31246/v1/api/weather?location=Berlin").And.UsingGet();
  }
}