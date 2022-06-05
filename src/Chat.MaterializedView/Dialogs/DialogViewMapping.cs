using Chat.InternalContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.MaterializedView.Dialogs
{
    internal class DialogViewMapping : IEntityTypeConfiguration<DialogView>
    {
        public void Configure(EntityTypeBuilder<DialogView> builder)
        {
            builder
                .ToTable("Dialogs")
                .HasKey(x => x.Id );

            builder.Property(x => x.Id);

            builder.Property(x => x.AggregateId)
                .HasColumnType("uniqueidentifier")
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(x => x.Name)
                .HasColumnType("nvarchar(max)")
                .HasMaxLength(50);
        }
    }
}
