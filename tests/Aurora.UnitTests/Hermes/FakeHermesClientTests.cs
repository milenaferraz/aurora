using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Contracts.Chat;
using Aurora.Infrastructure.Hermes;
using FluentAssertions;
using Xunit;

namespace Aurora.UnitTests.Hermes;

public class FakeHermesClientTests
{
    private readonly FakeHermesClient _sut = new();

    [Fact]
    public async Task StreamChatAsync_YieldsFourEventsInOrder()
    {
        var events = new List<ChatStreamEvent>();
        await foreach (var e in _sut.StreamChatAsync(new ChatRequest("hello"), CancellationToken.None))
            events.Add(e);

        events.Should().HaveCount(4);
        events[0].EventType.Should().Be("message.started");
        events[1].EventType.Should().Be("message.delta");
        events[2].EventType.Should().Be("message.delta");
        events[3].EventType.Should().Be("message.completed");
    }

    [Fact]
    public async Task ChatAsync_ReturnsNonEmptyConversationId()
    {
        var response = await _sut.ChatAsync(new ChatRequest("hello"), CancellationToken.None);
        response.ConversationId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task IsHealthyAsync_ReturnsTrue()
    {
        var result = await _sut.IsHealthyAsync(CancellationToken.None);
        result.Should().BeTrue();
    }
}
