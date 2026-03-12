using System.Reflection;
using Autofac;
using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Persistence.EfCore.DbContexts;
using BuildingBlocks.Persistence.EfCore.Interceptors;
using BuildingBlocks.Persistence.EfCore.Inbox;
using BuildingBlocks.Persistence.EfCore.Outbox;
using BuildingBlocks.Persistence.EfCore.Repositories;
using BuildingBlocks.Persistence.EfCore.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Persistence.EfCore;

public static class EfCoreAutofacExtensions
{
    extension(ContainerBuilder builder)
    {
        public ContainerBuilder AddDbContext<TDbContext>(Action<IComponentContext, DbContextOptionsBuilder<TDbContext>> configureAction)
            where TDbContext : EfCoreDbContext
        {
            builder.Register((context, _) =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
                configureAction(context, optionsBuilder);

                var interceptors = context.Resolve<IEnumerable<SaveChangesInterceptor>>();
                optionsBuilder.AddInterceptors(interceptors);

                return Activator.CreateInstance(typeof(TDbContext), optionsBuilder.Options) as TDbContext ??
                    throw new InvalidOperationException(DbContextMessages.CouldNotCreateInstance(typeof(TDbContext)));
            }).InstancePerLifetimeScope();

            return builder;
        }

        public ContainerBuilder AddReadRepositoriesFromAssemblies(params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies).Where(type => type.IsClosedTypeOf(typeof(EfCoreReadRepository<,,>))).AsImplementedInterfaces().InstancePerLifetimeScope();

            return builder;
        }


        public ContainerBuilder AddWriteRepositoriesFromAssemblies(params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies).Where(type => type.IsClosedTypeOf(typeof(EfCoreWriteRepository<,,>))).AsImplementedInterfaces().InstancePerLifetimeScope();

            return builder;
        }

        public ContainerBuilder AddUnitOfWorksFromAssemblies(params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies).Where(type => type.IsClosedTypeOf(typeof(EfCoreUnitOfWork<>))).AsImplementedInterfaces().InstancePerLifetimeScope();

            return builder;
        }

        public ContainerBuilder AddRepositoriesFromAssembly(params Assembly[] assemblies)
        {
            builder.AddReadRepositoriesFromAssemblies(assemblies);
            builder.AddWriteRepositoriesFromAssemblies(assemblies);
            builder.AddUnitOfWorksFromAssemblies(assemblies);
            return builder;
        }

        public ContainerBuilder AddDateTrackingInterceptor()
        {
            builder.RegisterType<DateTrackingInterceptor>().AsSelf().As<SaveChangesInterceptor>().InstancePerLifetimeScope().IfNotRegistered(typeof(DateTrackingInterceptor));

            return builder;
        }

        public ContainerBuilder AddAuditingInterceptor<TUserId>() where TUserId : struct
        {
            builder.RegisterType<AuditingInterceptor<TUserId>>().AsSelf().As<SaveChangesInterceptor>().InstancePerLifetimeScope().IfNotRegistered(typeof(AuditingInterceptor<TUserId>));

            return builder;
        }


        public ContainerBuilder AddOutboxInterceptor<TDbContext>() where TDbContext : EfCoreDbContext
        {
            builder.RegisterType<OutboxSaveChangesInterceptor<TDbContext>>().AsSelf().As<SaveChangesInterceptor>().InstancePerLifetimeScope();

            return builder;
        }

        public ContainerBuilder AddOutbox<TDbContext>() where TDbContext : EfCoreDbContext
        {
            builder.RegisterType<OutboxWriter<TDbContext>>().AsSelf().As<IOutboxWriter>().As<IOutboxMessageStore<TDbContext>>().InstancePerLifetimeScope();
            builder.AddOutboxInterceptor<TDbContext>();

            return builder;
        }

        public ContainerBuilder AddOutboxWritersFromAssemblies(params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies).Where(type => type.IsClosedTypeOf(typeof(OutboxWriter<>))).AsImplementedInterfaces().InstancePerLifetimeScope();

            return builder;
        }


        public ContainerBuilder AddInbox<TDbContext>() where TDbContext : EfCoreDbContext
        {
            builder.RegisterType<EfCoreInboxStore<TDbContext>>()
                .Keyed<IInboxStore>(typeof(TDbContext))
                .InstancePerLifetimeScope();

            return builder;
        }
    }
}