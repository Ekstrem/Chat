﻿// <auto-generated />
using System;
using Chat.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Chat.Storage.Migrations
{
    [DbContext(typeof(CommandDbContext))]
    [Migration("20220522115212_AddDbStore")]
    partial class AddDbStore
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Commands")
                .HasAnnotation("ProductVersion", "0.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Chat.Storage.DomainEventEntry", b =>
                {
                    b.Property<Guid>("AggregateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("AggregateVersion")
                        .HasColumnType("bigint");

                    b.Property<long>("Version")
                        .HasColumnType("bigint")
                        .HasColumnName("CommandVersion");

                    b.Property<string>("CommandName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("CorrelationToken")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Result")
                        .HasColumnType("int");

                    b.Property<string>("SubjectName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ValueObjects")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AggregateId", "AggregateVersion", "Version");

                    b.ToTable("DomainEvents", "Commands");
                });
#pragma warning restore 612, 618
        }
    }
}
