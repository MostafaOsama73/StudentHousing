using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context;

public class StudentHousingDBContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=StudentHousing;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudentHousingDBContext).Assembly); 
    }

    public DbSet<Booking> bookings { get; set; }
    public DbSet<Complaint> complaints { get; set; }
    public DbSet<HousingUnit> housingUnits { get; set; }
    public DbSet<LandLord> landLords { get; set; }
    public DbSet<Notification> notifications { get; set; }
    public DbSet<Payment> payments { get; set; }
    public DbSet<Review> reviews { get; set; }
    public DbSet<Room> rooms { get; set; }
    public DbSet<Student> students { get; set; }
    public DbSet<User> users { get; set; }
    public DbSet<Wishlist> wishlists { get; set; }

}
