using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    internal class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> B)
        {
            B.ToTable("Bookings");

            B.HasKey(B => B.BookingId);
            
            B.Property(B => B.StartDate)
                   .HasColumnName("Start Date")
                   .IsRequired();

            B.Property(B => B.EndDate)
                   .HasColumnName("End Date")
                   .IsRequired();

            B.Property(B => B.TotalPrice)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            B.Property(B => B.BookingStatus)
                   .IsRequired();


            B.HasOne(B => B.Student)
                   .WithMany(S => S.Bookings)
                   .HasForeignKey(B=> B.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            B.HasOne(B => B.Room)
                   .WithMany(R => R.Bookings)
                   .HasForeignKey(B => B.RoomId)
                   .OnDelete(DeleteBehavior.Restrict);

            B.HasOne(B => B.Payment)
              .WithOne(P => P.Booking)
              .HasForeignKey<Payment>(P => P.BookingId)
              .OnDelete(DeleteBehavior.Cascade);

            B.HasIndex(B => B.StudentId);
            B.HasIndex(B => B.RoomId);
            B.HasIndex(B => B.BookingStatus);
        }
    }
}
