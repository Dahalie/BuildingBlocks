using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Grpc.Interceptors;

public class ServerLoggingInterceptor(ILogger<ServerLoggingInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation("gRPC call started: {Method}", context.Method);

        try
        {
            var response = await continuation(request, context);
            stopwatch.Stop();
            logger.LogInformation("gRPC call completed: {Method} in {ElapsedMs}ms", context.Method, stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "gRPC call failed: {Method} after {ElapsedMs}ms", context.Method, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation("gRPC client-streaming call started: {Method}", context.Method);

        try
        {
            var response = await continuation(requestStream, context);
            stopwatch.Stop();
            logger.LogInformation("gRPC client-streaming call completed: {Method} in {ElapsedMs}ms", context.Method, stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "gRPC client-streaming call failed: {Method} after {ElapsedMs}ms", context.Method, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation("gRPC server-streaming call started: {Method}", context.Method);

        try
        {
            await continuation(request, responseStream, context);
            stopwatch.Stop();
            logger.LogInformation("gRPC server-streaming call completed: {Method} in {ElapsedMs}ms", context.Method, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "gRPC server-streaming call failed: {Method} after {ElapsedMs}ms", context.Method, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation("gRPC duplex-streaming call started: {Method}", context.Method);

        try
        {
            await continuation(requestStream, responseStream, context);
            stopwatch.Stop();
            logger.LogInformation("gRPC duplex-streaming call completed: {Method} in {ElapsedMs}ms", context.Method, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "gRPC duplex-streaming call failed: {Method} after {ElapsedMs}ms", context.Method, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
