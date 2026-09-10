using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
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
    private readonly string _baseUrl;

    public HermesHttpClient(IOptions<HermesOptions> options, IHttpClientFactory factory)
    {
        var opts = options.Value;
        if (string.IsNullOrWhiteSpace(opts.BaseUrl))
            throw new InvalidOperationException(
                "Hermes:BaseUrl must be configured when Hermes:UseFake is false.");

        _baseUrl = opts.BaseUrl.TrimEnd('/');
        _http = factory.CreateClient("hermes");
    }

    public async Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(request);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _http.PostAsync($"{_baseUrl}/api/chat", content, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new HermesException(
                $"Hermes returned {(int)response.StatusCode}",
                (int)response.StatusCode);

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ChatResponse>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async IAsyncEnumerable<ChatStreamEvent> StreamChatAsync(
        ChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(request);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/chat/stream")
        {
            Content = content
        };

        using var response = await _http.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        string? eventType = null;
        string? dataLine = null;

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line == null) break;

            if (line.StartsWith("event:"))
            {
                eventType = line.Substring(6).Trim();
            }
            else if (line.StartsWith("data:"))
            {
                dataLine = line.Substring(5).Trim();
            }
            else if (line == string.Empty && eventType != null && dataLine != null)
            {
                ChatStreamEvent? evt = null;
                try
                {
                    evt = JsonSerializer.Deserialize<ChatStreamEvent>(dataLine,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch { /* skip malformed */ }

                if (evt != null) yield return evt;
                eventType = null;
                dataLine = null;
            }
        }
    }

    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _http.GetAsync($"{_baseUrl}/health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
