using Aurora.Api.Middleware;
using Aurora.Domain.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Aurora.UnitTests.Middleware;

public class ExceptionHandlerMiddlewareTests
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger =
        Substitute.For<ILogger<ExceptionHandlerMiddleware>>();

    [Fact]
    public async Task InvokeAsync_UnhandledException_Returns500WithCorrelationId()
    {
        var context = new DefaultHttpContext();
        context.Items["CorrelationId"] = "test-id";
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlerMiddleware(
            _ => throw new InvalidOperationException("boom"),
            _logger);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(500);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        body.Should().Contain("An unexpected error occurred");
        body.Should().Contain("test-id");
    }

    [Fact]
    public async Task InvokeAsync_HermesException_Returns502()
    {
        var context = new DefaultHttpContext();
        context.Items["CorrelationId"] = "test-id";
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlerMiddleware(
            _ => throw new HermesException("Hermes down", 503),
            _logger);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(502);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        body.Should().Contain("Hermes is unreachable");
    }
}
