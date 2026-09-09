using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Contracts.Chat;

namespace Aurora.Application.Interfaces;

public interface IHermesClient
{
    Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken);
    IAsyncEnumerable<ChatStreamEvent> StreamChatAsync(ChatRequest request, CancellationToken cancellationToken);
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken);
}
