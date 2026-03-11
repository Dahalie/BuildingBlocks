using BuildingBlocks.Persistence.EfCore.Conventions;
using BuildingBlocks.Persistence.EfCore.Inbox;
using BuildingBlocks.Persistence.EfCore.Outbox;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.EfCore.DbContexts;

public class EfCoreDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Conventions.Add(_ => new GuidV7Convention());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
    }
}