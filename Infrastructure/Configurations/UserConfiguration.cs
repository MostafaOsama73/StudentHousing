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
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> U)
        {
            U.ToTable("Users");

            //U.Property(U => U.Name)
            //       .IsRequired()
            //       .HasColumnName("User Name")
            //       .HasMaxLength(150);

            //U.Property(U => U.AccountStatus)
            //       .IsRequired()
            //       .HasMaxLength(50);

            
            //U.Property(U => U.CreatedAt)
            //       .HasDefaultValueSql("getDate()")
            //       .IsRequired();

            U.Property(U => U.IsDeleted)
                   .HasDefaultValue(false);

            U.HasMany(U => U.Notifications)
                   .WithOne(N => N.User)
                   .HasForeignKey(N => N.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            U.HasQueryFilter(U => !U.IsDeleted);
        }
    }
}
