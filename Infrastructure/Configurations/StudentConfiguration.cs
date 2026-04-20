using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    internal class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> S)
        {
            S.ToTable("Students");
            
            S.HasKey(S => S.StudentId);

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

            // Timestamp columns
            S.Property(S => S.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            S.Property(S => S.UpdatedAt)
                   .IsRequired(false);

            // Verification properties
            S.Property(S => S.IsVerified)
                   .IsRequired()
                   .HasDefaultValue(false);

            S.Property(S => S.VerificationStatus)
                   .HasMaxLength(50)
                   .IsRequired(false);

            // One-to-One relationship with User
            S.HasOne(S => S.User)
                   .WithOne()
                   .HasForeignKey<Student>(S => S.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Collections
            // Bookings: Cascade (safe - no cycles)
            S.HasMany(S => S.Bookings)
                   .WithOne(B => B.Student)
                   .HasForeignKey(B => B.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Reviews: NO ACTION (prevents cascade cycle)
            S.HasMany(S => S.Reviews)
                   .WithOne(R => R.Student)
                   .HasForeignKey(R => R.StudentId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Complaints: NO ACTION (prevents cascade cycle)
            S.HasMany(S => S.Complaints)
                   .WithOne(C => C.Student)
                   .HasForeignKey(C => C.StudentId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Wishlists: NO ACTION (prevents cascade cycle)
            S.HasMany(S => S.Wishlists)
                   .WithOne(W => W.Student)
                   .HasForeignKey(W => W.StudentId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Indexes for performance
            S.HasIndex(S => S.UserId).IsUnique();
            S.HasIndex(S => S.City);
        }
    }
}
