using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Application.Interfaces;

namespace Aurora.Infrastructure.Voice;

public sealed class VoiceSpeechService : IVoiceSpeechService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _voiceId;
    private readonly string _modelId;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public VoiceSpeechService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("elevenlabs");
        _apiKey = Environment.GetEnvironmentVariable("ELEVENLABS_API_KEY")?.Trim() ?? string.Empty;
        _voiceId = Environment.GetEnvironmentVariable("ELEVENLABS_VOICE_ID")?.Trim() ?? string.Empty;
        _modelId = Environment.GetEnvironmentVariable("ELEVENLABS_MODEL_ID")?.Trim() ?? "eleven_multilingual_v2";
    }

    public async Task<byte[]> SpeakAsync(string text, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new InvalidOperationException("ELEVENLABS_API_KEY is not configured.");
        if (string.IsNullOrWhiteSpace(_voiceId))
            throw new InvalidOperationException("ELEVENLABS_VOICE_ID is not configured.");

        var payload = JsonSerializer.Serialize(new
        {
            text,
            model_id = _modelId,
        }, JsonOptions);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://api.elevenlabs.io/v1/text-to-speech/{_voiceId}?output_format=mp3_44100_128")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        request.Headers.Add("xi-api-key", _apiKey);

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"ElevenLabs returned {(int)response.StatusCode}: {error}");
        }

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }
}
