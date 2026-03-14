using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Storage
{
    internal class DomainEventMapping : IEntityTypeConfiguration<DomainEventEventEntry>
    {
        public void Configure(EntityTypeBuilder<DomainEventEventEntry> builder)
        {
            builder.ToTable("DomainEvents");

            builder.HasKey(x => new { x.Id, x.Version });

            builder.Property(x => x.CorrelationToken)
                .IsRequired();

            builder.Property(x => x.BoundedContext)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.CommandName)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.SubjectName)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.ChangedValueObjectsJson)
                .HasColumnType("jsonb");

            builder.Property(x => x.Result)
                .HasMaxLength(1024);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.CorrelationToken);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
