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
    internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> P)
        {
            P.ToTable("Payments");

            P.HasKey(P => P.PaymentId);

            P.Property(P => P.Amount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            P.Property(P => P.PaymentMethod)
                   .IsRequired();

            P.Property(P => P.PaymentStatus)
                   .IsRequired();

            P.Property(P => P.PaymentDate)
                   .HasDefaultValueSql("getDate()")
                   .IsRequired();

            P.Property(P => P.TransactionId)
                   .HasMaxLength(200);

            P.HasOne(P => P.Booking)
                   .WithOne(B => B.Payment)
                   .HasForeignKey<Booking>(P => P.BookingId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
