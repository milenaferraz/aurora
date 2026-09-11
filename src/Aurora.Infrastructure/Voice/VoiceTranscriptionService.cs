using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Application.Interfaces;
using Whisper.net;

namespace Aurora.Infrastructure.Voice;

public sealed class VoiceTranscriptionService : IVoiceTranscriptionService
{
    private readonly Lazy<WhisperFactory> _factory;
    private readonly string _modelPath;

    public VoiceTranscriptionService()
    {
        _modelPath = Environment.GetEnvironmentVariable("WHISPER_MODEL_PATH")?.Trim() ?? string.Empty;
        _factory = new Lazy<WhisperFactory>(() =>
        {
            if (string.IsNullOrWhiteSpace(_modelPath))
                throw new InvalidOperationException("WHISPER_MODEL_PATH is not configured.");
            if (!File.Exists(_modelPath))
                throw new FileNotFoundException($"Whisper model not found at '{_modelPath}'.", _modelPath);

            return WhisperFactory.FromPath(_modelPath);
        });
    }

    public async Task<string> TranscribeAsync(Stream audioStream, string? contentType, CancellationToken cancellationToken)
    {
        if (audioStream is null) throw new ArgumentNullException(nameof(audioStream));

        var inputExtension = GuessExtension(contentType);
        var tempInput = Path.Combine(Path.GetTempPath(), $"aurora-voice-{Guid.NewGuid():N}{inputExtension}");
        var tempWav = Path.Combine(Path.GetTempPath(), $"aurora-voice-{Guid.NewGuid():N}.wav");

        try
        {
            await using (var file = File.Create(tempInput))
            {
                await audioStream.CopyToAsync(file, cancellationToken);
            }

            await ConvertToWavAsync(tempInput, tempWav, cancellationToken);

            using var factory = _factory.Value;
            using var processor = factory.CreateBuilder().WithLanguage("auto").Build();
            await using var wavStream = File.OpenRead(tempWav);

            var text = new StringBuilder();
            await foreach (var segment in processor.ProcessAsync(wavStream))
            {
                if (string.IsNullOrWhiteSpace(segment.Text)) continue;
                if (text.Length > 0) text.Append(' ');
                text.Append(segment.Text.Trim());
            }

            return text.ToString().Trim();
        }
        finally
        {
            SafeDelete(tempInput);
            SafeDelete(tempWav);
        }
    }

    private static string GuessExtension(string? contentType) => contentType?.ToLowerInvariant() switch
    {
        "audio/wav" or "audio/x-wav" => ".wav",
        "audio/mpeg" or "audio/mp3" => ".mp3",
        "audio/ogg" => ".ogg",
        "audio/webm" or "audio/webm;codecs=opus" => ".webm",
        _ => ".bin",
    };

    private static async Task ConvertToWavAsync(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-y -i \"{inputPath}\" -ac 1 -ar 16000 -f wav \"{outputPath}\"",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(psi) ?? throw new InvalidOperationException("Unable to start ffmpeg.");
        var stderr = await process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"ffmpeg failed with exit code {process.ExitCode}: {stderr}");
    }

    private static void SafeDelete(string path)
    {
        try
        {
            if (File.Exists(path)) File.Delete(path);
        }
        catch
        {
            // best-effort cleanup
        }
    }
}
