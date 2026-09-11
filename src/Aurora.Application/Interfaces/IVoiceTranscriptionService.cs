using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Aurora.Application.Interfaces;

public interface IVoiceTranscriptionService
{
    Task<string> TranscribeAsync(Stream audioStream, string? contentType, CancellationToken cancellationToken);
}
