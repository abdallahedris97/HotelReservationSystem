
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User>? Users { get; set; }
    public DbSet<Room>? Rooms { get; set; }
    public DbSet<Reservation>? Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Seed data
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = "Admin"
        });
        modelBuilder.Entity<Room>().HasData(
            new Room { Id = 1, RoomNumber = "101", Capacity = 2, IsAvailable = true },
            new Room { Id = 2, RoomNumber = "102", Capacity = 2, IsAvailable = true }
        );
    }
}
