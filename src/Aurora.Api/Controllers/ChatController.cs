using System.Text.Json;
using Aurora.Application.Chat;
using Aurora.Contracts.Chat;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Aurora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;
    private readonly ChatStreamingService _chatStreamingService;

    public ChatController(ChatService chatService, ChatStreamingService chatStreamingService)
    {
        _chatService = chatService;
        _chatStreamingService = chatStreamingService;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Chat(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _chatService.ChatAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpOptions]
    public IActionResult Options()
    {
        return NoContent();
    }

    [HttpOptions("stream")]
    public IActionResult StreamOptions()
    {
        return NoContent();
    }

    [HttpPost("stream")]
    public async Task Stream([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        var feature = HttpContext.Features.Get<IHttpBodyControlFeature>();
        if (feature is not null)
            feature.AllowSynchronousIO = true;

        Response.ContentType = "text/event-stream; charset=utf-8";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["X-Accel-Buffering"] = "no";

        try
        {
            await foreach (var evt in _chatStreamingService.StreamAsync(request, cancellationToken))
            {
                var data = JsonSerializer.Serialize(evt, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                await Response.WriteAsync($"event: {evt.EventType}\ndata: {data}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Client disconnected
        }
        catch (Aurora.Domain.Exceptions.HermesException ex)
        {
            var errorData = JsonSerializer.Serialize(new { error = ex.Message });
            await Response.WriteAsync($"event: error\ndata: {errorData}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}
