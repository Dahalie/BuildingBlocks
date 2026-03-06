using BuildingBlocks.Primitives.Types;
using FluentAssertions;

namespace BuildingBlocks.Primitives.Tests.Types;

public class TypeExtensionsTests
{
    [Fact]
    public void MapToReadableString_SimpleType_ReturnsName()
    {
        typeof(string).MapToReadableString().Should().Be("String");
    }

    [Fact]
    public void MapToReadableString_GenericType_ReturnsReadableFormat()
    {
        typeof(List<int>).MapToReadableString().Should().Be("List<Int32>");
    }

    [Fact]
    public void MapToReadableString_NestedGeneric_ReturnsReadableFormat()
    {
        typeof(Dictionary<string, List<int>>).MapToReadableString()
            .Should().Be("Dictionary<String, List<Int32>>");
    }
}
