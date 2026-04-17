using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context;

public class StudentHousingDBContext : DbContext
{
    public StudentHousingDBContext(DbContextOptions<StudentHousingDBContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudentHousingDBContext).Assembly);
    }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Complaint> Complaints { get; set; }
    public DbSet<HousingUnit> HousingUnits { get; set; }
    public DbSet<LandLord> LandLords { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }

}
