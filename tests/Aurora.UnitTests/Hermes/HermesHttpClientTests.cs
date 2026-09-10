using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Application.Interfaces;
using Aurora.Contracts.Chat;
using Aurora.Domain.Exceptions;
using Aurora.Infrastructure.Hermes;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Aurora.UnitTests.Hermes;

public class HermesHttpClientTests
{
    private static HermesHttpClient CreateClient(
        HttpResponseMessage response,
        string baseUrl = "http://hermes.test")
    {
        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler);
        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient("hermes").Returns(httpClient);

        var options = Options.Create(new HermesOptions { BaseUrl = baseUrl });
        return new HermesHttpClient(options, factory);
    }

    [Fact]
    public async Task ChatAsync_NonSuccessStatus_ThrowsHermesException()
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadGateway);
        var sut = CreateClient(response);

        var act = async () => await sut.ChatAsync(
            new ChatRequest("hi"),
            CancellationToken.None);

        await act.Should().ThrowAsync<HermesException>()
            .Where(e => e.StatusCode == 502);
    }

    [Fact]
    public async Task IsHealthyAsync_HttpRequestException_ReturnsFalse()
    {
        var handler = new ThrowingHttpMessageHandler(new HttpRequestException("timeout"));
        var httpClient = new HttpClient(handler);
        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient("hermes").Returns(httpClient);

        var options = Options.Create(new HermesOptions { BaseUrl = "http://hermes.test" });
        var sut = new HermesHttpClient(options, factory);

        var result = await sut.IsHealthyAsync(CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public void Constructor_EmptyBaseUrl_ThrowsInvalidOperationException()
    {
        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient("hermes").Returns(new HttpClient());
        var options = Options.Create(new HermesOptions { BaseUrl = "" });

        var act = () => new HermesHttpClient(options, factory);

        act.Should().Throw<InvalidOperationException>();
    }
}

internal class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _response;

    public FakeHttpMessageHandler(HttpResponseMessage response)
    {
        _response = response;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
        => Task.FromResult(_response);
}

internal class ThrowingHttpMessageHandler : HttpMessageHandler
{
    private readonly Exception _ex;

    public ThrowingHttpMessageHandler(Exception ex)
    {
        _ex = ex;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
        => Task.FromException<HttpResponseMessage>(_ex);
}
