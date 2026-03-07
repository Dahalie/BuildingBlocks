using System.Reflection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace BuildingBlocks.Infrastructure.Jobs;

public static class QuartzAutofacExtensions
{
    public static ContainerBuilder AddQuartzWithJobs(
        this ContainerBuilder builder,
        IServiceCollection services,
        Assembly jobAssembly,
        IConfigurationSection? configSection = null,
        Action<QuartzHostingOptions>? configure = null)
    {
        var options = new QuartzHostingOptions();
        configSection?.Bind(options);
        configure?.Invoke(options);

        builder.RegisterInstance(options).AsSelf().SingleInstance();

        services.AddQuartz(q =>
        {
            QuartzMicrosoftExtensions.RegisterJobsFromAssembly(q, jobAssembly);
        });

        services.AddQuartzHostedService(q =>
        {
            q.WaitForJobsToComplete = options.WaitForJobsToComplete;
        });

        return builder;
    }
}
