using BuildingBlocks.Infrastructure.Grpc.Interceptors;
using FluentAssertions;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BuildingBlocks.Infrastructure.Tests.Grpc;

public class ClientLoggingInterceptorTests
{
    private readonly ClientLoggingInterceptor _interceptor;

    public ClientLoggingInterceptorTests()
    {
        var logger = Substitute.For<ILogger<ClientLoggingInterceptor>>();
        _interceptor = new ClientLoggingInterceptor(logger);
    }

    [Fact]
    public void ImplementsInterceptor()
    {
        _interceptor.Should().BeAssignableTo<Interceptor>();
    }

    [Fact]
    public async Task AsyncUnaryCall_SuccessfulCall_ReturnsResponse()
    {
        var context = CreateClientContext();

        var call = _interceptor.AsyncUnaryCall(
            "request", context,
            (req, ctx) => CreateUnaryCall(Task.FromResult("response")));

        var response = await call.ResponseAsync;
        response.Should().Be("response");
    }

    [Fact]
    public async Task AsyncUnaryCall_FailedCall_PropagatesException()
    {
        var context = CreateClientContext();

        var call = _interceptor.AsyncUnaryCall(
            "request", context,
            (req, ctx) => CreateUnaryCall(
                Task.FromException<string>(new RpcException(new Status(StatusCode.NotFound, "not found")))));

        var act = () => call.ResponseAsync;
        await act.Should().ThrowAsync<RpcException>();
    }

    [Fact]
    public async Task AsyncUnaryCall_PreservesResponseHeaders()
    {
        var expectedHeaders = new Metadata { { "x-custom", "value" } };
        var context = CreateClientContext();

        var call = _interceptor.AsyncUnaryCall(
            "request", context,
            (req, ctx) => new AsyncUnaryCall<string>(
                Task.FromResult("response"),
                Task.FromResult(expectedHeaders),
                () => new Status(StatusCode.OK, ""),
                () => [],
                () => { }));

        var headers = await call.ResponseHeadersAsync;
        headers.Should().BeSameAs(expectedHeaders);
    }

    private static AsyncUnaryCall<string> CreateUnaryCall(Task<string> responseTask)
    {
        return new AsyncUnaryCall<string>(
            responseTask,
            Task.FromResult(new Metadata()),
            () => new Status(StatusCode.OK, ""),
            () => [],
            () => { });
    }

    private static ClientInterceptorContext<string, string> CreateClientContext()
    {
        var marshaller = new Marshaller<string>(
            s => System.Text.Encoding.UTF8.GetBytes(s),
            b => System.Text.Encoding.UTF8.GetString(b));

        var method = new Method<string, string>(
            MethodType.Unary, "test.Service", "TestMethod", marshaller, marshaller);

        return new ClientInterceptorContext<string, string>(method, "localhost", new CallOptions());
    }
}
