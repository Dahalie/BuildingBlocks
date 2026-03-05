using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Persistence.EfCore.Inbox;

public class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("inbox_messages", "inbox");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MessageId).IsRequired();
        builder.Property(x => x.EventType).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Payload).IsRequired();
        builder.Property(x => x.ReceivedOn).IsRequired();
        builder.Property(x => x.ConsumerType).IsRequired().HasMaxLength(500);
        builder.Property(x => x.ErrorMessage).HasMaxLength(4000);

        builder.HasIndex(x => new { x.MessageId, x.ConsumerType })
               .IsUnique()
               .HasDatabaseName("IX_inbox_messages_message_consumer");

        builder.HasIndex(x => x.ProcessedOn)
               .HasFilter("\"ProcessedOn\" IS NULL")
               .HasDatabaseName("IX_inbox_messages_unprocessed");
    }
}
