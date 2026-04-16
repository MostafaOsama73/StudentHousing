using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    internal class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> R)
        {
            R.ToTable("Rooms");

            R.HasKey(R => R.RoomId);

            R.Property(R => R.RoomType)
                   .IsRequired();

            R.Property(R => R.NumberOfBeds)
                   .IsRequired();

            R.Property(R => R.HousingUnit)
                   .IsRequired();

            R.Property(R => R.Price)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            R.Property(R => R.IsAvailable)
                   .HasDefaultValue(true);

            R.HasOne(R => R.HousingUnit)
                   .WithMany(h => h.Rooms)
                   .HasForeignKey(R => R.HousingUnitId)
                   .OnDelete(DeleteBehavior.Cascade);

            R.HasMany(R => R.Bookings)
                   .WithOne(B => B.Room)
                   .HasForeignKey(B => B.RoomId)
                   .OnDelete(DeleteBehavior.Restrict);

            R.HasIndex(R => R.HousingUnitId);
        }
    }
}
