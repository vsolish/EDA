using Hotel.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Inventory.Infrastructure.Configuration;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.HotelId)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(r => r.RoomType)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(r => r.RoomNumber)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(r => r.PricePerNight)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.OwnsOne(r => r.Availability, av =>
        {
            av.Property(p => p.IsReserved)
                .HasColumnName("IsReserved")
                .IsRequired();

            av.Property(p => p.ReservationId)
                .HasColumnName("ReservationId");
        });

        builder.Navigation(r => r.Availability).IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt);

        builder.HasIndex(r => new { r.HotelId, r.RoomType })
            .HasDatabaseName("IX_Rooms_HotelId_RoomType");
    }
}