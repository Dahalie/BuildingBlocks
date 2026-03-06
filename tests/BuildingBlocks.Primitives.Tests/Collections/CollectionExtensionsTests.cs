using BuildingBlocks.Primitives.Collections;
using FluentAssertions;

namespace BuildingBlocks.Primitives.Tests.Collections;

public class CollectionExtensionsTests
{
    [Fact]
    public void RemoveAll_List_RemovesMatchingItems()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };

        var removed = list.RemoveAll(x => x > 3);

        removed.Should().Be(2);
        list.Should().BeEquivalentTo([1, 2, 3]);
    }

    [Fact]
    public void RemoveAll_HashSet_RemovesMatchingItems()
    {
        var set = new HashSet<int> { 1, 2, 3, 4, 5 };

        var removed = set.RemoveAll(x => x % 2 == 0);

        removed.Should().Be(2);
        set.Should().BeEquivalentTo([1, 3, 5]);
    }

    [Fact]
    public void RemoveAll_GenericCollection_RemovesMatchingItems()
    {
        ICollection<int> collection = new LinkedList<int>([1, 2, 3, 4]);

        var removed = collection.RemoveAll(x => x < 3);

        removed.Should().Be(2);
        collection.Should().BeEquivalentTo([3, 4]);
    }

    [Fact]
    public void AddRange_List_AddsAllItems()
    {
        ICollection<int> list = new List<int> { 1 };

        var added = list.AddRange(new[] { 2, 3, 4 });

        added.Should().Be(3);
        list.Should().BeEquivalentTo([1, 2, 3, 4]);
    }

    [Fact]
    public void AddRange_HashSet_ReturnsTrueAddedCount()
    {
        var set = new HashSet<int> { 1, 2 };

        var added = set.AddRange(new[] { 2, 3, 4 });

        added.Should().Be(2); // 2 is duplicate
        set.Should().BeEquivalentTo([1, 2, 3, 4]);
    }

    [Fact]
    public void AddRange_GenericCollection_AddsAllItems()
    {
        ICollection<int> collection = new LinkedList<int>([1]);

        var added = collection.AddRange(new[] { 2, 3 });

        added.Should().Be(2);
        collection.Should().BeEquivalentTo([1, 2, 3]);
    }
}
