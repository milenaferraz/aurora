using System.Threading;
using System.Threading.Tasks;

namespace Aurora.Application.Interfaces;

public interface IVoiceSpeechService
{
    Task<byte[]> SpeakAsync(string text, CancellationToken cancellationToken);
}
