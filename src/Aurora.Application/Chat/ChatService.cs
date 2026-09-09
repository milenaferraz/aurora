using System;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Application.Interfaces;
using Aurora.Contracts.Chat;

namespace Aurora.Application.Chat;

public class ChatService
{
    private readonly IHermesClient _hermesClient;

    public ChatService(IHermesClient hermesClient)
    {
        _hermesClient = hermesClient;
    }

    public async Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken)
    {
        var effectiveRequest = string.IsNullOrEmpty(request.ConversationId)
            ? request with { ConversationId = Guid.NewGuid().ToString() }
            : request;

        var response = await _hermesClient.ChatAsync(effectiveRequest, cancellationToken);

        return string.IsNullOrEmpty(response.ConversationId)
            ? response with { ConversationId = effectiveRequest.ConversationId! }
            : response;
    }
}
