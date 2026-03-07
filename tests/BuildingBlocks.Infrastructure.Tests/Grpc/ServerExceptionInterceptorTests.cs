using BuildingBlocks.Infrastructure.Grpc.Interceptors;
using BuildingBlocks.Primitives.Exceptions;
using BuildingBlocks.Primitives.Results;
using FluentAssertions;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BuildingBlocks.Infrastructure.Tests.Grpc;

public class ServerExceptionInterceptorTests
{
    private readonly ServerExceptionInterceptor _interceptor;
    private readonly ServerCallContext _context = new FakeServerCallContext();

    public ServerExceptionInterceptorTests()
    {
        var logger = Substitute.For<ILogger<ServerExceptionInterceptor>>();
        _interceptor = new ServerExceptionInterceptor(logger);
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
    public async Task UnaryServerHandler_RpcException_RethrowsAsIs()
    {
        var original = new RpcException(new Status(StatusCode.AlreadyExists, "exists"));

        var act = () => _interceptor.UnaryServerHandler<string, string>(
            "request", _context, (req, ctx) => throw original);

        var thrown = await act.Should().ThrowAsync<RpcException>();
        thrown.Which.Should().BeSameAs(original);
    }

    // ErrorType mapping tests

    [Theory]
    [InlineData(ErrorType.Validation,     StatusCode.InvalidArgument)]
    [InlineData(ErrorType.Authentication, StatusCode.Unauthenticated)]
    [InlineData(ErrorType.Authorization,  StatusCode.PermissionDenied)]
    [InlineData(ErrorType.NotFound,       StatusCode.NotFound)]
    [InlineData(ErrorType.Conflict,       StatusCode.AlreadyExists)]
    [InlineData(ErrorType.Business,       StatusCode.FailedPrecondition)]
    [InlineData(ErrorType.Exception,      StatusCode.Internal)]
    public async Task UnaryServerHandler_CustomException_MapsErrorTypeToStatusCode(ErrorType errorType, StatusCode expectedStatusCode)
    {
        var act = () => _interceptor.UnaryServerHandler<string, string>(
            "request", _context, (req, ctx) => throw new CustomException("test", errorType));

        var thrown = await act.Should().ThrowAsync<RpcException>();
        thrown.Which.StatusCode.Should().Be(expectedStatusCode);
        thrown.Which.Status.Detail.Should().Be("test");
    }

    [Fact]
    public async Task UnaryServerHandler_CustomExceptionWithoutErrorType_MapsToInternal()
    {
        var act = () => _interceptor.UnaryServerHandler<string, string>(
            "request", _context, (req, ctx) => throw new CustomException("unexpected"));

        var thrown = await act.Should().ThrowAsync<RpcException>();
        thrown.Which.StatusCode.Should().Be(StatusCode.Internal);
    }

    [Fact]
    public async Task UnaryServerHandler_ResultException_MapsErrorTypeToStatusCode()
    {
        var act = () => _interceptor.UnaryServerHandler<string, string>(
            "request", _context, (req, ctx) => throw new ResultException("not found", ErrorType.NotFound));

        var thrown = await act.Should().ThrowAsync<RpcException>();
        thrown.Which.StatusCode.Should().Be(StatusCode.NotFound);
    }

    // Standard .NET exception fallback tests

    [Fact]
    public async Task UnaryServerHandler_ArgumentException_MapsToInvalidArgument()
    {
        var act = () => _interceptor.UnaryServerHandler<string, string>(
            "request", _context, (req, ctx) => throw new ArgumentException("bad"));

        var thrown = await act.Should().ThrowAsync<RpcException>();
        thrown.Which.StatusCode.Should().Be(StatusCode.InvalidArgument);
    }

    [Fact]
    public async Task UnaryServerHandler_KeyNotFoundException_MapsToNotFound()
    {
        var act = () => _interceptor.UnaryServerHandler<string, string>(
            "request", _context, (req, ctx) => throw new KeyNotFoundException("missing"));

        var thrown = await act.Should().ThrowAsync<RpcException>();
        thrown.Which.StatusCode.Should().Be(StatusCode.NotFound);
    }

    [Fact]
    public async Task UnaryServerHandler_OperationCanceledException_MapsToCancelled()
    {
        var act = () => _interceptor.UnaryServerHandler<string, string>(
            "request", _context, (req, ctx) => throw new OperationCanceledException());

        var thrown = await act.Should().ThrowAsync<RpcException>();
        thrown.Which.StatusCode.Should().Be(StatusCode.Cancelled);
    }

    [Fact]
    public async Task UnaryServerHandler_UnknownException_MapsToInternal()
    {
        var act = () => _interceptor.UnaryServerHandler<string, string>(
            "request", _context, (req, ctx) => throw new Exception("boom"));

        var thrown = await act.Should().ThrowAsync<RpcException>();
        thrown.Which.StatusCode.Should().Be(StatusCode.Internal);
        thrown.Which.Status.Detail.Should().Be("An internal error occurred.");
    }

    // Streaming handler tests

    [Fact]
    public async Task ClientStreamingServerHandler_Exception_MapsToRpcException()
    {
        var requestStream = Substitute.For<IAsyncStreamReader<string>>();

        var act = () => _interceptor.ClientStreamingServerHandler<string, string>(
            requestStream, _context, (stream, ctx) => throw new CustomException("bad", ErrorType.Validation));

        var thrown = await act.Should().ThrowAsync<RpcException>();
        thrown.Which.StatusCode.Should().Be(StatusCode.InvalidArgument);
    }

    [Fact]
    public async Task ServerStreamingServerHandler_Exception_MapsToRpcException()
    {
        var responseStream = Substitute.For<IServerStreamWriter<string>>();

        var act = () => _interceptor.ServerStreamingServerHandler<string, string>(
            "request", responseStream, _context,
            (req, stream, ctx) => throw new CustomException("nope", ErrorType.Authorization));

        var thrown = await act.Should().ThrowAsync<RpcException>();
        thrown.Which.StatusCode.Should().Be(StatusCode.PermissionDenied);
    }

    [Fact]
    public async Task DuplexStreamingServerHandler_Exception_MapsToRpcException()
    {
        var requestStream = Substitute.For<IAsyncStreamReader<string>>();
        var responseStream = Substitute.For<IServerStreamWriter<string>>();

        var act = () => _interceptor.DuplexStreamingServerHandler<string, string>(
            requestStream, responseStream, _context,
            (reqStream, resStream, ctx) => throw new KeyNotFoundException("gone"));

        var thrown = await act.Should().ThrowAsync<RpcException>();
        thrown.Which.StatusCode.Should().Be(StatusCode.NotFound);
    }
}
