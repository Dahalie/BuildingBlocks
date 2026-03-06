using System.Reflection;
using Autofac;
using BuildingBlocks.Application.Mediator.Events;
using BuildingBlocks.Application.Mediator.Requests;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Application.Mediator;

public static class MediatorAutofacExtensions
{
    extension(ContainerBuilder builder)
    {
        public ContainerBuilder AddMediatR(Action<MediatRServiceConfiguration> configure)
        {
            var configuration = new MediatRServiceConfiguration();
            configure(configuration);

            builder.RegisterInstance(configuration).SingleInstance();

            builder.RegisterType<MediatR.Mediator>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            foreach (var descriptor in configuration.BehaviorsToRegister)
            {
                builder.RegisterGeneric(descriptor.ImplementationType!)
                    .As(typeof(IPipelineBehavior<,>))
                    .InstancePerDependency();
            }

            builder.AddEventDispatcher();
            builder.AddRequestDispatcher();
            return builder;
        }

        public ContainerBuilder AddRequestHandlersFromAssemblies(params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                .AsClosedTypesOf(typeof(IRequestHandler<,>))
                .InstancePerDependency();

            return builder;
        }

        public ContainerBuilder AddEventHandlersFromAssemblies(params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                .AsClosedTypesOf(typeof(INotificationHandler<>))
                .InstancePerDependency();

            return builder;
        }

        public ContainerBuilder AddValidatorsFromAssemblies(params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                .AsClosedTypesOf(typeof(IValidator<>))
                .InstancePerDependency();

            return builder;
        }

        private void AddEventDispatcher()
        {
            builder.RegisterType<EventDispatcher>().As<IEventDispatcher>().InstancePerDependency().IfNotRegistered(typeof(IEventDispatcher));
        }

        private void AddRequestDispatcher()
        {
            builder.RegisterType<RequestDispatcher>().As<IRequestDispatcher>().InstancePerDependency().IfNotRegistered(typeof(IRequestDispatcher));
        }
    }
}
