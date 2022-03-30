﻿// <auto-generated />
using System;
using Infrastructure.EventStore.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.EventStore.Migrations
{
    [DbContext(typeof(EventStoreDbContext))]
    [Migration("20220330224103_Quartz Migration")]
    partial class QuartzMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-preview.2.22153.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Application.EventSourcing.EventStore.Events.PaymentSnapshot", b =>
                {
                    b.Property<long>("AggregateVersion")
                        .HasColumnType("bigint");

                    b.Property<Guid>("AggregateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AggregateName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("varchar(30)");

                    b.Property<string>("AggregateState")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AggregateVersion", "AggregateId");

                    b.ToTable("PaymentSnapshots");
                });

            modelBuilder.Entity("Application.EventSourcing.EventStore.Events.PaymentStoreEvent", b =>
                {
                    b.Property<long>("Version")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Version"), 1L, 1);

                    b.Property<Guid>("AggregateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AggregateName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Event")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Version");

                    b.ToTable("PaymentStoreEvents");
                });
#pragma warning restore 612, 618
        }
    }
}
