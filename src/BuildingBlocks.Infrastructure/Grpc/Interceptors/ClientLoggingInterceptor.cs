using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Grpc.Interceptors;

public class ClientLoggingInterceptor(ILogger<ClientLoggingInterceptor> logger) : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var methodName = context.Method.FullName;
        logger.LogInformation("gRPC client call started: {Method}", methodName);
        var stopwatch = Stopwatch.StartNew();

        var call = continuation(request, context);

        return new AsyncUnaryCall<TResponse>(
            HandleResponseAsync(call.ResponseAsync, methodName, stopwatch),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);
    }

    public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        var methodName = context.Method.FullName;
        logger.LogInformation("gRPC client streaming call started: {Method}", methodName);
        var stopwatch = Stopwatch.StartNew();

        var call = continuation(context);

        return new AsyncClientStreamingCall<TRequest, TResponse>(
            call.RequestStream,
            HandleResponseAsync(call.ResponseAsync, methodName, stopwatch),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);
    }

    public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        var methodName = context.Method.FullName;
        logger.LogInformation("gRPC server streaming call started: {Method}", methodName);

        return continuation(request, context);
    }

    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        var methodName = context.Method.FullName;
        logger.LogInformation("gRPC duplex streaming call started: {Method}", methodName);

        return continuation(context);
    }

    private async Task<TResponse> HandleResponseAsync<TResponse>(
        Task<TResponse> responseTask, string method, Stopwatch stopwatch)
    {
        try
        {
            var response = await responseTask;
            stopwatch.Stop();
            logger.LogInformation("gRPC client call completed: {Method} in {ElapsedMs}ms", method, stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "gRPC client call failed: {Method} after {ElapsedMs}ms", method, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
