using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Persistence.EfCore.Outbox;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages", "outbox");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventType).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Payload).IsRequired();
        builder.Property(x => x.OccurredOn).IsRequired();
        builder.Property(x => x.ErrorMessage).HasMaxLength(4000);

        builder.HasIndex(x => x.ProcessedOn)
               .HasFilter("\"ProcessedOn\" IS NULL")
               .HasDatabaseName("IX_outbox_messages_unprocessed");
    }
}
