using Chat.InternalContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Storage
{
    internal class DomainEventMapping : IEntityTypeConfiguration<DomainEventEntry>
    {
        public void Configure(EntityTypeBuilder<DomainEventEntry> builder)
        {
            builder
                .ToTable("DomainEvents")
                .HasKey(x => new { x.AggregateId, x.AggregateVersion, x.Version });

            builder.Property(x => x.AggregateId)
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(x => x.AggregateVersion)
                //.HasValueGenerator<VersionGenerator>()
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(x => x.Version)
                .HasColumnType("bigint")
                .HasColumnName("CommandVersion")
                .IsRequired();

            builder.Property(x => x.CommandName)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50);

            builder.Property(x => x.SubjectName)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50);

            builder.Property(x => x.ValueObjects)
                .HasColumnType("nvarchar(max)");

            builder.Ignore(x => x.Command);
            //builder.Ignore(x => x.ChangedValueObjects);
        }
    }
}
