using BuildingBlocks.Infrastructure.Jobs;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;

namespace BuildingBlocks.Infrastructure.Tests.Jobs;

public class JobBaseTests
{
    private sealed class SuccessfulJob(ILogger logger) : JobBase(logger)
    {
        public bool Executed { get; private set; }

        protected override Task ExecuteAsync(CancellationToken ct)
        {
            Executed = true;
            return Task.CompletedTask;
        }
    }

    private sealed class FailingJob(ILogger logger) : JobBase(logger)
    {
        protected override Task ExecuteAsync(CancellationToken ct)
        {
            throw new InvalidOperationException("Something went wrong");
        }
    }

    private sealed class CancellingJob(ILogger logger) : JobBase(logger)
    {
        protected override Task ExecuteAsync(CancellationToken ct)
        {
            throw new OperationCanceledException();
        }
    }

    [Fact]
    public async Task Execute_SuccessfulJob_CompletesWithoutError()
    {
        var job = new SuccessfulJob(NullLogger.Instance);

        await job.Execute(new FakeJobExecutionContext());

        job.Executed.Should().BeTrue();
    }

    [Fact]
    public async Task Execute_FailingJob_WrapsInJobExecutionException()
    {
        var job = new FailingJob(NullLogger.Instance);

        var act = () => job.Execute(new FakeJobExecutionContext());

        await act.Should().ThrowAsync<JobExecutionException>()
            .Where(ex => ex.InnerException is InvalidOperationException);
    }

    [Fact]
    public async Task Execute_CancellingJob_ThrowsOperationCancelled()
    {
        var job = new CancellingJob(NullLogger.Instance);

        var act = () => job.Execute(new FakeJobExecutionContext());

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
