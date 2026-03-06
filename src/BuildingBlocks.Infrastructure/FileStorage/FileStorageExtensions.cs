using Autofac;
using BuildingBlocks.Application.FileStorage;

namespace BuildingBlocks.Infrastructure.FileStorage;

public static class FileStorageExtensions
{
    public static void AddFileStorage(this ContainerBuilder builder)
    {
        builder.RegisterType<LocalFileStorage>().As<IFileStorage>().InstancePerLifetimeScope();
    }
}
