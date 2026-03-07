using BuildingBlocks.Infrastructure.Grpc.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Grpc;

public static class GrpcMicrosoftExtensions
{
    public static IServiceCollection AddGrpcInterceptors(this IServiceCollection services)
    {
        services.AddSingleton<ServerExceptionInterceptor>();
        services.AddSingleton<ServerLoggingInterceptor>();
        services.AddSingleton<ClientLoggingInterceptor>();

        return services;
    }
}
