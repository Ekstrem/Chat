using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Storage.ReadModels
{
    internal class ChatReadModelConfiguration : IEntityTypeConfiguration<ChatReadModel>
    {
        public void Configure(EntityTypeBuilder<ChatReadModel> builder)
        {
            builder.ToTable("ChatReadModels");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(x => x.SubscriberName)
                .HasMaxLength(256);

            builder.Property(x => x.OperatorName)
                .HasMaxLength(256);

            builder.Property(x => x.LastCommandName)
                .HasMaxLength(256);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.UpdatedAt);
        }
    }
}
