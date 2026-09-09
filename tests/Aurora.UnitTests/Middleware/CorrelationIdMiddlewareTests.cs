using Aurora.Api.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Aurora.UnitTests.Middleware;

public class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_NoHeader_GeneratesNewCorrelationId()
    {
        var context = new DefaultHttpContext();
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        context.Items["CorrelationId"].Should().NotBeNull();
        context.Response.Headers["X-Correlation-Id"].ToString().Should().NotBeEmpty();
    }

    [Fact]
    public async Task InvokeAsync_WithHeader_EchoesSameId()
    {
        var existingId = "test-correlation-id";
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-Id"] = existingId;
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        context.Items["CorrelationId"].Should().Be(existingId);
        context.Response.Headers["X-Correlation-Id"].ToString().Should().Be(existingId);
    }
}
