using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Application.Interfaces;
using Aurora.Contracts.Chat;

namespace Aurora.Infrastructure.Hermes;

public class FakeHermesClient : IHermesClient
{
    public async Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken)
    {
        await Task.Delay(200, cancellationToken);
        var conversationId = string.IsNullOrEmpty(request.ConversationId)
            ? Guid.NewGuid().ToString()
            : request.ConversationId;
        return new ChatResponse(conversationId, "Boa noite! ✨\n\nEstou online e pronta para ajudar.");
    }

    public async IAsyncEnumerable<ChatStreamEvent> StreamChatAsync(
        ChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return new ChatStreamEvent("message.started");
        await Task.Delay(50, cancellationToken);
        yield return new ChatStreamEvent("message.delta", Content: "Boa noite! ✨");
        await Task.Delay(50, cancellationToken);
        yield return new ChatStreamEvent("message.delta", Content: "\n\nEstou online e pronta para ajudar.");
        await Task.Delay(50, cancellationToken);
        yield return new ChatStreamEvent("message.completed", ConversationId: Guid.NewGuid().ToString());
    }

    public Task<bool> IsHealthyAsync(CancellationToken cancellationToken)
        => Task.FromResult(true);
}
