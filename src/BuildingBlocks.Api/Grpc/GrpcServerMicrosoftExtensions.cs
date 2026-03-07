using BuildingBlocks.Infrastructure.Grpc.Interceptors;
using Grpc.AspNetCore.Server;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Api.Grpc;

public static class GrpcServerMicrosoftExtensions
{
    public static IServiceCollection AddGrpcWithInterceptors(
        this IServiceCollection services,
        Action<GrpcServiceOptions>? configure = null)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<ServerExceptionInterceptor>();
            options.Interceptors.Add<ServerLoggingInterceptor>();
            configure?.Invoke(options);
        });

        return services;
    }
}
