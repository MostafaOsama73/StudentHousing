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
    internal class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> R)
        {
            R.ToTable("Reviews");

            R.HasKey(r => r.ReviewId);

            R.Property(r => r.Rating)
                   .IsRequired();

            R.Property(r => r.Comment)
                   .HasMaxLength(1000);

            R.Property(r => r.ReviewDate)
                   .IsRequired();

            R.HasOne(r => r.Student)
                   .WithMany(s => s.Reviews)
                   .HasForeignKey(r => r.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            R.HasOne(r => r.HousingUnit)
                   .WithMany(h => h.Reviews)
                   .HasForeignKey(r => r.HousingUnitId)
                   .OnDelete(DeleteBehavior.Cascade);

            R.HasIndex(r => r.StudentId);
            R.HasIndex(r => r.HousingUnitId);
        }
    }
}
