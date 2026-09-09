using Aurora.Domain.Exceptions;
using System.Text.Json;

namespace Aurora.Api.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (HermesException ex)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? "unknown";
            _logger.LogError(ex, "Hermes error. CorrelationId: {CorrelationId}", correlationId);

            context.Response.StatusCode = 502;
            context.Response.ContentType = "application/json";
            var body = JsonSerializer.Serialize(new { error = "Hermes is unreachable", correlationId });
            await context.Response.WriteAsync(body);
        }
        catch (Exception ex)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? "unknown";
            _logger.LogError(ex, "Unhandled exception. CorrelationId: {CorrelationId}", correlationId);

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var body = JsonSerializer.Serialize(new { error = "An unexpected error occurred", correlationId });
            await context.Response.WriteAsync(body);
        }
    }
}
