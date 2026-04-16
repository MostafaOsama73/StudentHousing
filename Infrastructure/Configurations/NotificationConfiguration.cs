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
    internal class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(N => N.NotificationId);

            builder.Property(N => N.Message)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(N => N.Type)
                   .IsRequired()
                   .HasColumnName("Notification Type")
                   .HasMaxLength(100);

            builder.Property(N => N.IsSeen)
                   .HasDefaultValue(false);

            builder.Property(N => N.CreatedAt)
                   .IsRequired();

            builder.HasOne(N => N.User)
                   .WithMany(U => U.Notifications)
                   .HasForeignKey(N => N.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
