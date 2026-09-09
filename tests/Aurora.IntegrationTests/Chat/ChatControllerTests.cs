using System.Net;
using System.Net.Http.Json;
using Aurora.Contracts.Chat;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Aurora.IntegrationTests.Chat;

public class ChatControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ChatControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_ValidMessage_Returns200WithConversationId()
    {
        var request = new ChatRequest("Aurora, boa noite.");
        var response = await _client.PostAsJsonAsync("/api/chat", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ChatResponse>();
        body!.ConversationId.Should().NotBeNullOrEmpty();
        body.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Post_EmptyMessage_Returns400()
    {
        var request = new ChatRequest("");
        var response = await _client.PostAsJsonAsync("/api/chat", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_ValidMessage_ResponseHasCorrelationIdHeader()
    {
        var request = new ChatRequest("hello");
        var response = await _client.PostAsJsonAsync("/api/chat", request);

        response.Headers.Contains("X-Correlation-Id").Should().BeTrue();
    }
}
