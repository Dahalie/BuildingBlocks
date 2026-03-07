using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace BuildingBlocks.Infrastructure.Jobs;

public static class QuartzMicrosoftExtensions
{
    public static IServiceCollection AddQuartzWithJobs(
        this IServiceCollection services,
        Assembly jobAssembly,
        IConfigurationSection? configSection = null,
        Action<QuartzHostingOptions>? configure = null)
    {
        var options = new QuartzHostingOptions();
        configSection?.Bind(options);
        configure?.Invoke(options);

        services.AddSingleton(options);

        services.AddQuartz(q =>
        {
            RegisterJobsFromAssembly(q, jobAssembly);
        });

        services.AddQuartzHostedService(q =>
        {
            q.WaitForJobsToComplete = options.WaitForJobsToComplete;
        });

        return services;
    }

    internal static void RegisterJobsFromAssembly(IServiceCollectionQuartzConfigurator q, Assembly assembly)
    {
        var jobTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsClass: true }
                         && t.GetCustomAttribute<CronJobAttribute>() is not null);

        foreach (var jobType in jobTypes)
        {
            var attr = jobType.GetCustomAttribute<CronJobAttribute>()!;
            RegisterJob(q, jobType, attr);
        }
    }

    private static void RegisterJob(
        IServiceCollectionQuartzConfigurator q,
        Type jobType,
        CronJobAttribute attr)
    {
        var identity = attr.Identity ?? jobType.Name;
        var jobKey = new JobKey(identity);

        q.AddJob(jobType, jobKey, opts =>
        {
            if (attr.DisallowConcurrentExecution)
                opts.DisallowConcurrentExecution();
        });

        q.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity($"{identity}-trigger")
            .WithCronSchedule(attr.CronExpression));
    }
}
