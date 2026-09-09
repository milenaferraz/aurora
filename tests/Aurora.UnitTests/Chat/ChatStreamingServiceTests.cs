using Aurora.Application.Chat;
using Aurora.Application.Interfaces;
using Aurora.Contracts.Chat;
using FluentAssertions;
using NSubstitute;

namespace Aurora.UnitTests.Chat;

public class ChatStreamingServiceTests
{
    private readonly IHermesClient _hermes = Substitute.For<IHermesClient>();
    private readonly ChatStreamingService _sut;

    public ChatStreamingServiceTests()
    {
        _sut = new ChatStreamingService(_hermes);
    }

    [Fact]
    public async Task StreamAsync_NullConversationId_AssignsNewUuid()
    {
        ChatRequest? capturedRequest = null;
        _hermes.StreamChatAsync(Arg.Do<ChatRequest>(r => capturedRequest = r), Arg.Any<CancellationToken>())
            .Returns(AsyncEnumerable(new ChatStreamEvent("message.started")));

        await foreach (var _ in _sut.StreamAsync(new ChatRequest("hello"), CancellationToken.None)) { }

        capturedRequest!.ConversationId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task StreamAsync_ProvidedConversationId_PassesThroughUnchanged()
    {
        var id = "existing-id";
        ChatRequest? capturedRequest = null;
        _hermes.StreamChatAsync(Arg.Do<ChatRequest>(r => capturedRequest = r), Arg.Any<CancellationToken>())
            .Returns(AsyncEnumerable(new ChatStreamEvent("message.started")));

        await foreach (var _ in _sut.StreamAsync(new ChatRequest("hello", id), CancellationToken.None)) { }

        capturedRequest!.ConversationId.Should().Be(id);
    }

    private static async IAsyncEnumerable<ChatStreamEvent> AsyncEnumerable(params ChatStreamEvent[] events)
    {
        foreach (var e in events)
        {
            yield return e;
            await Task.Yield();
        }
    }
}
