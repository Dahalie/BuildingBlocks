using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.EfCore.Inbox;

public static class InboxModelBuilderExtensions
{
    public static ModelBuilder ApplyInboxConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        return modelBuilder;
    }
}
