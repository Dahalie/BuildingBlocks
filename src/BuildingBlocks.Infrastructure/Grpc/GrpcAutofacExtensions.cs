using Autofac;
using BuildingBlocks.Infrastructure.Grpc.Interceptors;

namespace BuildingBlocks.Infrastructure.Grpc;

public static class GrpcAutofacExtensions
{
    public static ContainerBuilder AddGrpcInterceptors(this ContainerBuilder builder)
    {
        builder.RegisterType<ServerExceptionInterceptor>().AsSelf().SingleInstance();
        builder.RegisterType<ServerLoggingInterceptor>().AsSelf().SingleInstance();
        builder.RegisterType<ClientLoggingInterceptor>().AsSelf().SingleInstance();

        return builder;
    }
}
