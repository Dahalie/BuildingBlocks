using BuildingBlocks.Domain.Entities;
using FluentAssertions;

namespace BuildingBlocks.Domain.Tests.Entities;

public class TestEntity : IEntity<Guid>, IDateTrackable, IAuditable<Guid>
{
    public Guid            Id        { get; set; }
    public DateTimeOffset  CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid            CreatedBy { get; set; }
    public Guid?           UpdatedBy { get; set; }
}

public class EntityInterfaceTests
{
    [Fact]
    public void Entity_ImplementsAllInterfaces()
    {
        var entity = new TestEntity();

        entity.Should().BeAssignableTo<IEntity>();
        entity.Should().BeAssignableTo<IEntity<Guid>>();
        entity.Should().BeAssignableTo<IDateTrackable>();
        entity.Should().BeAssignableTo<IAuditable<Guid>>();
    }

    [Fact]
    public void Entity_PropertiesAreSettable()
    {
        var id = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var userId = Guid.NewGuid();

        var entity = new TestEntity
        {
            Id        = id,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        entity.Id.Should().Be(id);
        entity.CreatedAt.Should().Be(now);
        entity.UpdatedAt.Should().Be(now);
        entity.CreatedBy.Should().Be(userId);
        entity.UpdatedBy.Should().Be(userId);
    }

    [Fact]
    public void Entity_NullableProperties_DefaultToNull()
    {
        var entity = new TestEntity();

        entity.UpdatedAt.Should().BeNull();
        entity.UpdatedBy.Should().BeNull();
    }
}
