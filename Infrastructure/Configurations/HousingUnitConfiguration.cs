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
    internal class HousingUnitConfiguration : IEntityTypeConfiguration<HousingUnit>
    {
        public void Configure(EntityTypeBuilder<HousingUnit> HU)
        {
            HU.ToTable("HousingUnits");

            HU.HasKey(HU => HU.HousingUnitId);

            HU.Property(HU => HU.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            HU.Property(HU => HU.Description)
                   .IsRequired()
                   .HasMaxLength(1000);

            HU.Property(HU => HU.Address)
                   .IsRequired()
                   .HasMaxLength(250);

            HU.Property(HU => HU.City)
                   .IsRequired()
                   .HasMaxLength(100);

            HU.Property(HU => HU.Area)
                   .IsRequired()
                   .HasMaxLength(100);

            HU.Property(HU => HU.Price)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            HU.Property(HU => HU.Rules)
                   .IsRequired()
                   .HasMaxLength(1000);

            HU.Property(HU => HU.IsAvailable)
                   .HasDefaultValue(true);


            HU.HasOne(HU => HU.LandLord)
                   .WithMany()
                   .HasForeignKey(HU => HU.LandLordId)
                   .OnDelete(DeleteBehavior.Cascade);

            HU.HasMany(HU => HU.Rooms)
                   .WithOne(R => R.HousingUnit)
                   .HasForeignKey(R => R.HousingUnitId)
                   .OnDelete(DeleteBehavior.Cascade);

            HU.HasMany(HU => HU.Reviews)
                   .WithOne(r => r.HousingUnit)
                   .HasForeignKey(r => r.HousingUnitId)
                   .OnDelete(DeleteBehavior.Cascade);

            HU.HasMany(HU => HU.WishlistedBy)
                   .WithOne(w => w.HousingUnit)
                   .HasForeignKey(w => w.HousingUnitId)
                   .OnDelete(DeleteBehavior.Cascade);

            HU.HasIndex(HU => HU.City);
            HU.HasIndex(HU => HU.Area);
            HU.HasIndex(HU => HU.Price);
        }
    }
}
