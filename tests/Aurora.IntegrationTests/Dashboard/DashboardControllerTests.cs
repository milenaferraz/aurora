using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Aurora.Contracts.Dashboard;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Aurora.IntegrationTests.Dashboard;

public class DashboardControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DashboardControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Dashboard_Returns200WithAllRequiredFields()
    {
        var response = await _client.GetAsync("/api/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<DashboardResponse>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        body.Should().NotBeNull();
        body!.Greeting.Should().NotBeNullOrEmpty();
        body.Aurora.Should().NotBeNull();
        body.Agenda.Should().NotBeNull();
        body.Tasks.Should().NotBeNull();
        body.Emails.Should().NotBeNull();
        body.System.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_Dashboard_SystemHermesIsOnlineWithFakeClient()
    {
        var response = await _client.GetAsync("/api/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<DashboardResponse>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        body!.System.Hermes.Should().Be("online");
    }

    [Fact]
    public async Task Get_Health_Returns200WithBothEntries()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();

        content.Should().Contain("aurora-api");
        content.Should().Contain("hermes");
        content.Should().Contain("Healthy");
    }
}
