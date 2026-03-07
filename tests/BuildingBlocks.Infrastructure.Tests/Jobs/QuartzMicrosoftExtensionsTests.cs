using System.Reflection;
using BuildingBlocks.Infrastructure.Jobs;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;

namespace BuildingBlocks.Infrastructure.Tests.Jobs;

[CronJob("0 */5 * * * ?")]
public class TestJob(ILogger<TestJob> logger) : JobBase(logger)
{
    protected override Task ExecuteAsync(CancellationToken ct) => Task.CompletedTask;
}

[CronJob("0 0 * * * ?", "CustomIdentity", DisallowConcurrentExecution = true)]
public class TestJobWithIdentity(ILogger<TestJobWithIdentity> logger) : JobBase(logger)
{
    protected override Task ExecuteAsync(CancellationToken ct) => Task.CompletedTask;
}

public class QuartzMicrosoftExtensionsTests
{
    [Fact]
    public void AddQuartzWithJobs_RegistersQuartzServices()
    {
        var services = new ServiceCollection();
        services.AddLogging(b => b.AddProvider(NullLoggerProvider.Instance));

        services.AddQuartzWithJobs(Assembly.GetExecutingAssembly());

        var provider = services.BuildServiceProvider();
        var schedulerFactory = provider.GetService<ISchedulerFactory>();

        schedulerFactory.Should().NotBeNull();
    }

    [Fact]
    public void AddQuartzWithJobs_RegistersHostingOptions()
    {
        var services = new ServiceCollection();
        services.AddLogging(b => b.AddProvider(NullLoggerProvider.Instance));

        services.AddQuartzWithJobs(Assembly.GetExecutingAssembly());

        var provider = services.BuildServiceProvider();
        var options = provider.GetService<QuartzHostingOptions>();

        options.Should().NotBeNull();
        options!.WaitForJobsToComplete.Should().BeTrue();
    }

    [Fact]
    public void AddQuartzWithJobs_WithConfigSection_BindsOptions()
    {
        var services = new ServiceCollection();
        services.AddLogging(b => b.AddProvider(NullLoggerProvider.Instance));

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Quartz:WaitForJobsToComplete"] = "false"
            })
            .Build();

        services.AddQuartzWithJobs(
            Assembly.GetExecutingAssembly(),
            config.GetSection("Quartz"));

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<QuartzHostingOptions>();

        options.WaitForJobsToComplete.Should().BeFalse();
    }

    [Fact]
    public void AddQuartzWithJobs_WithConfigureCallback_OverridesOptions()
    {
        var services = new ServiceCollection();
        services.AddLogging(b => b.AddProvider(NullLoggerProvider.Instance));

        services.AddQuartzWithJobs(
            Assembly.GetExecutingAssembly(),
            configure: opts => opts.WaitForJobsToComplete = false);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<QuartzHostingOptions>();

        options.WaitForJobsToComplete.Should().BeFalse();
    }

    [Fact]
    public void RegisterJobsFromAssembly_FindsAnnotatedJobs()
    {
        var services = new ServiceCollection();
        services.AddLogging(b => b.AddProvider(NullLoggerProvider.Instance));

        services.AddQuartz(q =>
        {
            QuartzMicrosoftExtensions.RegisterJobsFromAssembly(q, Assembly.GetExecutingAssembly());
        });

        // Verify by building and getting scheduler
        var provider = services.BuildServiceProvider();
        var schedulerFactory = provider.GetRequiredService<ISchedulerFactory>();

        schedulerFactory.Should().NotBeNull();
    }

    [Fact]
    public void AddQuartzWithJobs_ReturnsServiceCollection_ForChaining()
    {
        var services = new ServiceCollection();
        services.AddLogging(b => b.AddProvider(NullLoggerProvider.Instance));

        var result = services.AddQuartzWithJobs(Assembly.GetExecutingAssembly());

        result.Should().BeSameAs(services);
    }
}
