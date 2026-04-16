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
    internal class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> W)
        {
            W.ToTable("Wishlists");

            W.HasKey(W => W.WishlistId);

            W.Property(W => W.AddedDate)
                   .IsRequired();

         
            W.HasOne(W => W.Student)
                   .WithMany(s => s.Wishlists)
                   .HasForeignKey(W => W.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            W.HasOne(W => W.HousingUnit)
                   .WithMany(H => H.WishlistedBy)
                   .HasForeignKey(W => W.HousingUnitId)
                   .OnDelete(DeleteBehavior.Cascade);

            
        }
    }
}
