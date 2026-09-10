using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Application.Interfaces;
using Aurora.Contracts.Chat;
using Aurora.Domain.Exceptions;
using Microsoft.Extensions.Options;

namespace Aurora.Infrastructure.Hermes;

public class HermesHttpClient : IHermesClient
{
    private readonly HttpClient _http;
    private readonly HermesOptions _opts;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public HermesHttpClient(IOptions<HermesOptions> options, IHttpClientFactory factory)
    {
        _opts = options.Value;
        if (string.IsNullOrWhiteSpace(_opts.BaseUrl))
            throw new InvalidOperationException(
                "Hermes:BaseUrl must be configured when Hermes:UseFake is false.");

        _http = factory.CreateClient("hermes");

        if (!string.IsNullOrWhiteSpace(_opts.ApiKey))
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _opts.ApiKey);
    }

    public async Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(BuildPayload(request, stream: false), JsonOpts);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync(
            $"{_opts.BaseUrl.TrimEnd('/')}/v1/chat/completions", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new HermesException(
                $"Hermes returned {(int)response.StatusCode}",
                (int)response.StatusCode);

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var completion = JsonSerializer.Deserialize<OaiCompletionResponse>(body, JsonOpts)
            ?? throw new HermesException("Empty response from Hermes", 500);

        var text = completion.Choices?[0].Message?.Content ?? string.Empty;
        return new ChatResponse(request.ConversationId ?? Guid.NewGuid().ToString(), text);
    }

    public async IAsyncEnumerable<ChatStreamEvent> StreamChatAsync(
        ChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(BuildPayload(request, stream: true), JsonOpts);
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_opts.BaseUrl.TrimEnd('/')}/v1/chat/completions")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        using var response = await _http.SendAsync(
            httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new HermesException(
                $"Hermes stream returned {(int)response.StatusCode}",
                (int)response.StatusCode);

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);
        var conversationId = request.ConversationId ?? Guid.NewGuid().ToString();

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line == null) break;
            if (!line.StartsWith("data:")) continue;

            var data = line[5..].Trim();
            if (data == "[DONE]")
            {
                yield return new ChatStreamEvent("done", ConversationId: conversationId);
                yield break;
            }

            OaiStreamChunk? chunk = null;
            try { chunk = JsonSerializer.Deserialize<OaiStreamChunk>(data, JsonOpts); }
            catch { continue; }

            var delta = chunk?.Choices?[0].Delta;
            if (delta == null) continue;

            if (!string.IsNullOrEmpty(delta.ToolCallId))
                yield return new ChatStreamEvent("tool_call", Tool: delta.ToolCallId, ConversationId: conversationId);
            else if (delta.Content != null)
                yield return new ChatStreamEvent("token", Content: delta.Content, ConversationId: conversationId);
        }
    }

    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _http.GetAsync($"{_opts.BaseUrl.TrimEnd('/')}/health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    private OaiChatRequest BuildPayload(ChatRequest request, bool stream) => new(
        Model: _opts.Model,
        Messages: [new OaiMessage("user", request.Message)],
        Stream: stream);

    // OpenAI-compatible DTOs

    private record OaiChatRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("messages")] OaiMessage[] Messages,
        [property: JsonPropertyName("stream")] bool Stream);

    private record OaiMessage(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("content")] string Content);

    private record OaiCompletionResponse(
        [property: JsonPropertyName("choices")] OaiChoice[]? Choices);

    private record OaiChoice(
        [property: JsonPropertyName("message")] OaiMessage? Message);

    private record OaiStreamChunk(
        [property: JsonPropertyName("choices")] OaiStreamChoice[]? Choices);

    private record OaiStreamChoice(
        [property: JsonPropertyName("delta")] OaiDelta? Delta);

    private record OaiDelta(
        [property: JsonPropertyName("content")] string? Content,
        [property: JsonPropertyName("tool_call_id")] string? ToolCallId);
}
