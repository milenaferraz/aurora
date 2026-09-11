using Aurora.Application.Interfaces;
using Aurora.Contracts.Voice;
using Microsoft.AspNetCore.Mvc;

namespace Aurora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VoiceController : ControllerBase
{
    private readonly IVoiceTranscriptionService _transcriptionService;
    private readonly IVoiceSpeechService _speechService;

    public VoiceController(
        IVoiceTranscriptionService transcriptionService,
        IVoiceSpeechService speechService)
    {
        _transcriptionService = transcriptionService;
        _speechService = speechService;
    }

    [HttpOptions("transcribe")]
    public IActionResult TranscribeOptions()
    {
        return NoContent();
    }

    [HttpPost("transcribe")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<VoiceTranscribeResponse>> Transcribe(
        [FromForm] IFormFile audio,
        CancellationToken cancellationToken)
    {
        if (audio is null || audio.Length == 0)
            return BadRequest(new { error = "Audio file is required." });

        await using var stream = audio.OpenReadStream();
        var text = await _transcriptionService.TranscribeAsync(stream, audio.ContentType, cancellationToken);
        return Ok(new VoiceTranscribeResponse(text));
    }

    [HttpOptions("speak")]
    public IActionResult SpeakOptions()
    {
        return NoContent();
    }

    [HttpPost("speak")]
    public async Task<IActionResult> Speak(
        [FromBody] VoiceSpeakRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Text is required." });

        var audio = await _speechService.SpeakAsync(request.Text, cancellationToken);
        return File(audio, "audio/mpeg", "aurora-voice.mp3");
    }
}
