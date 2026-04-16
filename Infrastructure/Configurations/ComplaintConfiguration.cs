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
    internal class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
    {
        public void Configure(EntityTypeBuilder<Complaint> C)
        {
            C.ToTable("Complaints");

            C.HasKey(C => C.ComplaintId);

            C.Property(C => C.Description)
                   .IsRequired()
                   .HasMaxLength(1000);

            C.Property(C => C.Status)
                   .IsRequired();

            C.Property(C => C.CreatedDate)
                   .IsRequired();

            C.HasOne(C => C.Student)
                   .WithMany(S => S.Complaints)
                   .HasForeignKey(C => C.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            C.HasOne(C => C.LandLord)
                   .WithMany()
                   .HasForeignKey(C => C.LandLordId)
                   .OnDelete(DeleteBehavior.Restrict);

            C.HasIndex(C => C.StudentId);
            C.HasIndex(C => C.LandLordId);
            C.HasIndex(C => C.Status);
        }
    }
}
