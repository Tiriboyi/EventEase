﻿// <auto-generated />
using System;
using EventEase.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EventEase.Migrations
{
    [DbContext(typeof(EventEaseContext))]
    partial class EventEaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EventEase.Models.Booking", b =>
                {
                    b.Property<int>("BookingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BookingId"));

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<int>("VenueId")
                        .HasColumnType("int");

                    b.HasKey("BookingId");

                    b.HasIndex("EventId");

                    b.HasIndex("VenueId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("EventEase.Models.Event", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EventId"));

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("EventDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("EventTypeId")
                        .HasColumnType("int");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VenueId")
                        .HasColumnType("int");

                    b.HasKey("EventId");

                    b.HasIndex("EventTypeId");

                    b.HasIndex("VenueId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("EventEase.Models.EventType", b =>
                {
                    b.Property<int>("EventTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EventTypeId"));

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("EventTypeId");

                    b.ToTable("EventTypes");

                    b.HasData(
                        new
                        {
                            EventTypeId = 1,
                            TypeName = "Conference"
                        },
                        new
                        {
                            EventTypeId = 2,
                            TypeName = "Wedding"
                        },
                        new
                        {
                            EventTypeId = 3,
                            TypeName = "Concert"
                        });
                });

            modelBuilder.Entity("EventEase.Models.Venue", b =>
                {
                    b.Property<int>("VenueId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VenueId"));

                    b.Property<bool>("Availability")
                        .HasColumnType("bit");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<int?>("EventTypeId")
                        .HasColumnType("int");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("VenueName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("VenueId");

                    b.HasIndex("EventTypeId");

                    b.ToTable("Venues");
                });

            modelBuilder.Entity("EventEase.Models.Booking", b =>
                {
                    b.HasOne("EventEase.Models.Event", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EventEase.Models.Venue", "Venue")
                        .WithMany("Bookings")
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("EventEase.Models.Event", b =>
                {
                    b.HasOne("EventEase.Models.EventType", "EventType")
                        .WithMany()
                        .HasForeignKey("EventTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EventEase.Models.Venue", "Venue")
                        .WithMany()
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("EventType");

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("EventEase.Models.Venue", b =>
                {
                    b.HasOne("EventEase.Models.EventType", null)
                        .WithMany("Venues")
                        .HasForeignKey("EventTypeId");
                });

            modelBuilder.Entity("EventEase.Models.EventType", b =>
                {
                    b.Navigation("Venues");
                });

            modelBuilder.Entity("EventEase.Models.Venue", b =>
                {
                    b.Navigation("Bookings");
                });
#pragma warning restore 612, 618
        }
    }
}
