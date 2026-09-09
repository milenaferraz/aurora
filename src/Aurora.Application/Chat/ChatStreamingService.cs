using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Aurora.Application.Interfaces;
using Aurora.Contracts.Chat;

namespace Aurora.Application.Chat;

public class ChatStreamingService
{
    private readonly IHermesClient _hermesClient;

    public ChatStreamingService(IHermesClient hermesClient)
    {
        _hermesClient = hermesClient;
    }

    public async IAsyncEnumerable<ChatStreamEvent> StreamAsync(
        ChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var effectiveRequest = string.IsNullOrEmpty(request.ConversationId)
            ? request with { ConversationId = Guid.NewGuid().ToString() }
            : request;

        await foreach (var evt in _hermesClient.StreamChatAsync(effectiveRequest, cancellationToken))
        {
            yield return evt;
        }
    }
}
