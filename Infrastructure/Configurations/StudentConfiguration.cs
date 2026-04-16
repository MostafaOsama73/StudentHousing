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
    internal class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> S)
        {
            S.HasBaseType<User>();

            S.Property(S => S.DateOfBirth)
                   .IsRequired();

            S.Property(S => S.Gender)
                   .IsRequired();

            S.Property(S => S.Address)
                   .IsRequired()
                   .HasMaxLength(200);

            S.Property(S => S.City)
                   .IsRequired()
                   .HasMaxLength(100);

            S.Property(S => S.PreferredArea)
                   .HasMaxLength(100);

            S.Property(S => S.NationalId)
                   .IsRequired()
                   .HasMaxLength(50);


            S.HasMany(S => S.Bookings)
                   .WithOne(B => B.Student)
                   .HasForeignKey(B => B.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            S.HasMany(S => S.Reviews)
                   .WithOne(R => R.Student)
                   .HasForeignKey(R => R.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            S.HasMany(S => S.Complaints)
                   .WithOne(C => C.Student)
                   .HasForeignKey(C => C.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            S.HasMany(S => S.Wishlists)
                   .WithOne(W => W.Student)
                   .HasForeignKey(W => W.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
