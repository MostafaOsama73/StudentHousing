using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    internal class LandlordConfiguration : IEntityTypeConfiguration<LandLord>
    {
        public void Configure(EntityTypeBuilder<LandLord> L)
        {
            L.ToTable("LandLords");
            
            L.HasKey(L => L.LandLordId);

            L.Property(L => L.CompanyName)
                   .HasColumnName("Company Name")
                   .HasMaxLength(150)
                   .IsRequired(false);

            L.Property(L => L.NationalId)
                   .IsRequired()
                   .HasMaxLength(50);

            L.Property(L => L.PropertyOwnerShipProof)
                   .IsRequired()
                   .HasMaxLength(250);

            // Verification status
            L.Property(L => L.VerificationStatus)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasConversion<string>();

            // Timestamp columns
            L.Property(L => L.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            L.Property(L => L.UpdatedAt)
                   .IsRequired(false);

            L.Property(L => L.ApprovedAt)
                   .IsRequired(false);

            L.Property(L => L.RejectionReason)
                   .HasMaxLength(500)
                   .IsRequired(false);

            // One-to-One relationship with User
            L.HasOne(L => L.User)
                   .WithOne()
                   .HasForeignKey<LandLord>(L => L.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Collections
            L.HasMany(L => L.HousingUnits)
                   .WithOne(HU => HU.LandLord)
                   .HasForeignKey(HU => HU.LandLordId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            L.HasIndex(L => L.UserId).IsUnique();
            L.HasIndex(L => L.NationalId).IsUnique();
        }
    }
}
