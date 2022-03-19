//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chat.Domain;
//using Chat.Domain.Abstraction;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace Chat.Storage
//{
//    internal class SlobMapping : IEntityTypeConfiguration<SlobEntry<IChatAnemicModel, IChat>>
//    {
//        public void Configure(EntityTypeBuilder<SlobEntry<IChatAnemicModel, IChat>> builder)
//        {
//            builder.ToTable("Aggregate")
//                .HasKey(x => x.Id);

//            builder.Property(x => x.Id)
//                .HasColumnType("uniqueidentifier")
//                .IsRequired();

//            builder.Property(x => x.Version)
//                .HasValueGenerator<VersionGenerator>()
//                .IsRequired()
//                .ValueGeneratedNever();

//            builder.Property(x => x.CorrelationToken)
//                .HasColumnType("uniqueidentifier")
//                .IsRequired();

//            builder.Property(x => x.SubjectName)
//                .HasColumnType("nvarchar(50)")
//                .HasMaxLength(50);

//            builder.Property(x => x.CommandName)
//                .HasColumnType("nvarchar(50)")
//                .HasMaxLength(50);

//            builder.Property(x => x.ValueObjects);
//        }
//    }
//}
