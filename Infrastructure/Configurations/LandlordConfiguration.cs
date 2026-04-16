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
    internal class LandlordConfiguration : IEntityTypeConfiguration<LandLord>
    {
        public void Configure(EntityTypeBuilder<LandLord> L)
        {
            L.HasBaseType<User>();

            L.Property(L => L.CompanyName)
                   .IsRequired()
                   .HasColumnName("Company Name")
                   .HasMaxLength(150);

            L.Property(L => L.NationalId)
                   .IsRequired()
                   .HasMaxLength(50);

            L.Property(L => L.PropertyOwnerShipProof)
                   .IsRequired()
                   .HasMaxLength(250);

            L.Property(L => L.VerificationStatus)
                   .IsRequired()
                   .HasMaxLength(50);
        }
    }
}
