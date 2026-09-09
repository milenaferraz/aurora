using System.Net;
using System.Net.Http.Json;
using Aurora.Contracts.Chat;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Aurora.IntegrationTests.Chat;

public class ChatStreamControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ChatStreamControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostStream_ValidMessage_ReturnsEventStream()
    {
        var request = new ChatRequest("Aurora, boa noite.");
        var content = JsonContent.Create(request);

        var response = await _client.PostAsync("/api/chat/stream", content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/event-stream");
    }

    [Fact]
    public async Task PostStream_ValidMessage_BodyContainsStartedAndCompletedEvents()
    {
        var request = new ChatRequest("hello");
        var content = JsonContent.Create(request);

        var response = await _client.PostAsync("/api/chat/stream", content);
        var body = await response.Content.ReadAsStringAsync();

        body.Should().Contain("message.started");
        body.Should().Contain("message.completed");
    }
}
