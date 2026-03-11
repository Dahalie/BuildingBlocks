using BuildingBlocks.Persistence.EfCore.ValueGenerators;
using FluentAssertions;

namespace BuildingBlocks.Persistence.Tests.ValueGenerators;

public class GuidV7ValueGeneratorTests
{
    [Fact]
    public void Next_ReturnsNonEmptyGuid()
    {
        var generator = new GuidV7ValueGenerator();

        var result = generator.Next(null!);

        result.Should().NotBeEmpty();
    }

    [Fact]
    public void Next_ReturnsUniqueValues()
    {
        var generator = new GuidV7ValueGenerator();

        var first  = generator.Next(null!);
        var second = generator.Next(null!);

        first.Should().NotBe(second);
    }

    [Fact]
    public void GeneratesTemporaryValues_ReturnsFalse()
    {
        var generator = new GuidV7ValueGenerator();

        generator.GeneratesTemporaryValues.Should().BeFalse();
    }
}
