using System.Net;
using System.Text.Json;
using FluentAssertions;
using TestLibrary.S3427.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;
using WireMock.FluentAssertions;
using WireMock.Matchers;

namespace IntegrationTests.S9662;

public class ApiTestsS6962: IDisposable
{
  private readonly WireMockServer _wireMockServer;
  public ApiTestsS6962()
  {
    _wireMockServer = WireMockServer.Start(31246);
  }

  [Fact]
  public async Task GetWithDefaultValues_ShouldReturnSuccessfully()
  {
    // Arrange
    SetupWireMockServer();
    await using var factory = new WebApplicationFactory();
    var client = factory.CreateClient();

    // Act
    var response = await client.GetAsync($"/S6962/external");

    // Assert
    response.EnsureSuccessStatusCode();
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var content = await JsonSerializer.DeserializeAsync<List<WeatherModelCelsius>>(await response.Content.ReadAsStreamAsync(), JsonSerializerOptions.Web);
    content.Should().NotBeNull();
    content!.Count.Should().Be(3);
    content.Should().Satisfy(w => w.Temperature == 20, w => w.Temperature == 15, w => w.Temperature == 25);

    _wireMockServer.Should().HaveReceivedACall().AtUrl("http://localhost:31246/api/weather").And.UsingGet();
  }

  [Fact]
  public async Task GetWithDefaultValues_NoResults_ShouldReturnNotFound()
  {
    // Arrange
    SetupWireMockServer();
    await using var factory = new WebApplicationFactory();
    var client = factory.CreateClient();

    // Act
    var response = await client.GetAsync($"/S6962/external?argument=Berlin");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    _wireMockServer.Should().HaveReceivedACall().AtUrl("http://localhost:31246/api/weather?location=Berlin").And.UsingGet();
  }

  [Fact]
  public async Task GetWithDefaultValues_ErrorDuringRequest_ShouldReturnBadRequest()
  {
    // Arrange
    SetupWireMockServer();
    await using var factory = new WebApplicationFactory();
    var client = factory.CreateClient();

    // Act
    var response = await client.GetAsync($"/S6962/external?argument=Madrid");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    _wireMockServer.Should().HaveReceivedACall().AtUrl("http://localhost:31246/api/weather?location=Madrid").And.UsingGet();
  }


  private void SetupWireMockServer()
  {
    _wireMockServer.Given(Request.Create().UsingGet().WithPath(path => path.EndsWith("weather")).WithParam("location", MatchBehaviour.RejectOnMatch))
                   .RespondWith(Response.Create().WithStatusCode(200).WithBody("""
                                                                               [
                                                                                 {
                                                                                    "temperature": 20
                                                                                 },
                                                                                 {
                                                                                  "temperature": 15
                                                                                  },
                                                                                  {
                                                                                  "temperature": 25
                                                                                  }
                                                                               ]
                                                                               """));

    _wireMockServer.Given(Request.Create().WithParam("location", MatchBehaviour.AcceptOnMatch, "Madrid"))
                   .RespondWith(Response.Create().WithStatusCode(404));

    _wireMockServer.Given(Request.Create().UsingGet().WithParam("location", MatchBehaviour.AcceptOnMatch, "Berlin"))
                   .RespondWith(Response.Create().WithStatusCode(200).WithBody("""
                                                                               [
                                                                                 
                                                                               ]
                                                                               """));
  }

  public void Dispose()
  {
    _wireMockServer.Dispose();
  }
}