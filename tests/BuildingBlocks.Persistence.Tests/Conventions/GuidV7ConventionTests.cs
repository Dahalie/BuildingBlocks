using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Persistence.EfCore.Conventions;
using BuildingBlocks.Persistence.EfCore.DbContexts;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Persistence.Tests.Conventions;

public class GuidV7ConventionTests
{
    private GuidV7TestDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<GuidV7TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new GuidV7TestDbContext(options);
    }

    [Fact]
    public async Task AddedEntity_WithEmptyGuid_GetsGuidV7Assigned()
    {
        using var dbContext = CreateDbContext();
        var entity = new TestGuidEntity { Name = "Test" };

        entity.Id.Should().BeEmpty();

        dbContext.TestEntities.Add(entity);
        await dbContext.SaveChangesAsync();

        entity.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AddedEntity_WithExplicitId_KeepsOriginalId()
    {
        using var dbContext = CreateDbContext();
        var explicitId = Guid.CreateVersion7();
        var entity = new TestGuidEntity { Id = explicitId, Name = "Test" };

        dbContext.TestEntities.Add(entity);
        await dbContext.SaveChangesAsync();

        entity.Id.Should().Be(explicitId);
    }

    [Fact]
    public async Task MultipleEntities_GetUniqueIds()
    {
        using var dbContext = CreateDbContext();
        var entity1 = new TestGuidEntity { Name = "First" };
        var entity2 = new TestGuidEntity { Name = "Second" };

        dbContext.TestEntities.AddRange(entity1, entity2);
        await dbContext.SaveChangesAsync();

        entity1.Id.Should().NotBeEmpty();
        entity2.Id.Should().NotBeEmpty();
        entity1.Id.Should().NotBe(entity2.Id);
    }

    [Fact]
    public async Task IntPrimaryKey_IsNotAffectedByConvention()
    {
        using var dbContext = CreateDbContext();
        var entity = new TestIntEntity { Name = "Test" };

        dbContext.TestIntEntities.Add(entity);
        await dbContext.SaveChangesAsync();

        entity.Id.Should().BeGreaterThan(0);
    }
}

public class TestGuidEntity : IEntity<Guid>
{
    public Guid   Id   { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TestIntEntity : IEntity<int>
{
    public int    Id   { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class GuidV7TestDbContext(DbContextOptions<GuidV7TestDbContext> options) : EfCoreDbContext(options)
{
    public DbSet<TestGuidEntity> TestEntities    => Set<TestGuidEntity>();
    public DbSet<TestIntEntity>  TestIntEntities => Set<TestIntEntity>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.AddGuidV7Convention();
    }
}
