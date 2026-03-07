using BuildingBlocks.Primitives.Exceptions;
using BuildingBlocks.Primitives.Results;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Grpc.Interceptors;

public class ServerExceptionInterceptor(ILogger<ServerExceptionInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw CreateRpcException(ex, context.Method);
        }
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(requestStream, context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw CreateRpcException(ex, context.Method);
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(request, responseStream, context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw CreateRpcException(ex, context.Method);
        }
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(requestStream, responseStream, context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw CreateRpcException(ex, context.Method);
        }
    }

    private RpcException CreateRpcException(Exception ex, string method)
    {
        var (statusCode, message) = MapException(ex);
        logger.LogError(ex, "gRPC error {StatusCode} for {Method}", statusCode, method);
        return new RpcException(new Status(statusCode, message));
    }

    internal static (StatusCode StatusCode, string Message) MapException(Exception ex)
    {
        if (ex is CustomException customException)
            return MapErrorType(customException.ErrorType, customException.Message);

        return ex switch
        {
            ArgumentException           => (StatusCode.InvalidArgument, ex.Message),
            UnauthorizedAccessException => (StatusCode.PermissionDenied, ex.Message),
            KeyNotFoundException        => (StatusCode.NotFound, ex.Message),
            OperationCanceledException  => (StatusCode.Cancelled, "Operation was cancelled."),
            NotImplementedException     => (StatusCode.Unimplemented, ex.Message),
            InvalidOperationException   => (StatusCode.FailedPrecondition, ex.Message),
            _                           => (StatusCode.Internal, "An internal error occurred.")
        };
    }

    internal static (StatusCode StatusCode, string Message) MapErrorType(ErrorType errorType, string message)
    {
        var statusCode = errorType switch
        {
            ErrorType.Validation     => StatusCode.InvalidArgument,
            ErrorType.Authentication => StatusCode.Unauthenticated,
            ErrorType.Authorization  => StatusCode.PermissionDenied,
            ErrorType.NotFound       => StatusCode.NotFound,
            ErrorType.Conflict       => StatusCode.AlreadyExists,
            ErrorType.Business       => StatusCode.FailedPrecondition,
            _                        => StatusCode.Internal
        };

        return (statusCode, message);
    }
}
