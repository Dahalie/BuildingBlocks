namespace BuildingBlocks.Infrastructure.Jobs;

public class QuartzHostingOptions
{
    public bool WaitForJobsToComplete { get; set; } = true;
}
