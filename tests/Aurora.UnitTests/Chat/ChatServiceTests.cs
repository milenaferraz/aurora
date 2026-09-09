using System.Threading;
using System.Threading.Tasks;
using Aurora.Application.Chat;
using Aurora.Application.Interfaces;
using Aurora.Contracts.Chat;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Aurora.UnitTests.Chat;

public class ChatServiceTests
{
    private readonly IHermesClient _hermes = Substitute.For<IHermesClient>();
    private readonly ChatService _sut;

    public ChatServiceTests()
    {
        _sut = new ChatService(_hermes);
    }

    [Fact]
    public async Task ChatAsync_NullConversationId_AssignsNewUuid()
    {
        var capturedRequest = default(ChatRequest);
        _hermes.ChatAsync(Arg.Do<ChatRequest>(r => capturedRequest = r), Arg.Any<CancellationToken>())
            .Returns(ci => new ChatResponse(((ChatRequest)ci[0]).ConversationId!, "reply"));

        var result = await _sut.ChatAsync(new ChatRequest("hello"), CancellationToken.None);

        capturedRequest!.ConversationId.Should().NotBeNullOrEmpty();
        result.ConversationId.Should().Be(capturedRequest.ConversationId);
    }

    [Fact]
    public async Task ChatAsync_ProvidedConversationId_PassesThroughUnchanged()
    {
        var id = "my-conversation-id";
        _hermes.ChatAsync(Arg.Any<ChatRequest>(), Arg.Any<CancellationToken>())
            .Returns(new ChatResponse(id, "reply"));

        var result = await _sut.ChatAsync(new ChatRequest("hello", id), CancellationToken.None);

        result.ConversationId.Should().Be(id);
    }
}
