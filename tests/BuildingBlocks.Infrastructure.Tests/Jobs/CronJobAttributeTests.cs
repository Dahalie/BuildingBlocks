using BuildingBlocks.Infrastructure.Jobs;
using FluentAssertions;

namespace BuildingBlocks.Infrastructure.Tests.Jobs;

public class CronJobAttributeTests
{
    [Fact]
    public void Constructor_WithCronExpression_SetsProperties()
    {
        var attr = new CronJobAttribute("0 */5 * * * ?");

        attr.CronExpression.Should().Be("0 */5 * * * ?");
        attr.Identity.Should().BeNull();
        attr.DisallowConcurrentExecution.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithIdentity_SetsIdentity()
    {
        var attr = new CronJobAttribute("0 0 * * * ?", "MyJob");

        attr.Identity.Should().Be("MyJob");
    }

    [Fact]
    public void DisallowConcurrentExecution_CanBeSet()
    {
        var attr = new CronJobAttribute("0 0 * * * ?")
        {
            DisallowConcurrentExecution = true
        };

        attr.DisallowConcurrentExecution.Should().BeTrue();
    }
}
