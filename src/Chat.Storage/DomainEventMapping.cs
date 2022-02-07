using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }
    }
}
