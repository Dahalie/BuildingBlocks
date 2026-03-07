using BuildingBlocks.Infrastructure.Grpc.Interceptors;
using FluentAssertions;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BuildingBlocks.Infrastructure.Tests.Grpc;

public class ServerLoggingInterceptorTests
{
    private readonly ServerLoggingInterceptor _interceptor;
    private readonly ServerCallContext _context = new FakeServerCallContext();

    public ServerLoggingInterceptorTests()
    {
        var logger = Substitute.For<ILogger<ServerLoggingInterceptor>>();
        _interceptor = new ServerLoggingInterceptor(logger);
    }

    [Fact]
    public void ImplementsInterceptor()
    {
        _interceptor.Should().BeAssignableTo<Interceptor>();
    }

    [Fact]
    public async Task UnaryServerHandler_SuccessfulCall_ReturnsResponse()
    {
        var response = await _interceptor.UnaryServerHandler<string, string>(
            "request", _context, (req, ctx) => Task.FromResult("response"));

        response.Should().Be("response");
    }

    [Fact]
    public async Task UnaryServerHandler_FailedCall_RethrowsException()
    {
        var act = () => _interceptor.UnaryServerHandler<string, string>(
            "request", _context, (req, ctx) => throw new InvalidOperationException("fail"));

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("fail");
    }

    [Fact]
    public async Task ClientStreamingServerHandler_SuccessfulCall_ReturnsResponse()
    {
        var requestStream = Substitute.For<IAsyncStreamReader<string>>();

        var response = await _interceptor.ClientStreamingServerHandler<string, string>(
            requestStream, _context, (stream, ctx) => Task.FromResult("streamed"));

        response.Should().Be("streamed");
    }

    [Fact]
    public async Task ServerStreamingServerHandler_SuccessfulCall_Completes()
    {
        var responseStream = Substitute.For<IServerStreamWriter<string>>();

        var act = () => _interceptor.ServerStreamingServerHandler<string, string>(
            "request", responseStream, _context, (req, stream, ctx) => Task.CompletedTask);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DuplexStreamingServerHandler_SuccessfulCall_Completes()
    {
        var requestStream = Substitute.For<IAsyncStreamReader<string>>();
        var responseStream = Substitute.For<IServerStreamWriter<string>>();

        var act = () => _interceptor.DuplexStreamingServerHandler<string, string>(
            requestStream, responseStream, _context, (reqStream, resStream, ctx) => Task.CompletedTask);

        await act.Should().NotThrowAsync();
    }
}
