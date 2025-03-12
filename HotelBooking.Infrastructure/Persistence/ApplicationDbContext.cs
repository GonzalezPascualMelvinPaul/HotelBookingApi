using Microsoft.EntityFrameworkCore;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }  
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Email = "admin@example.com", PasswordHash = "admin123", Role = "Admin" },
                new User { Id = 2, Username = "user", Email = "user@example.com", PasswordHash = "user123", Role = "Client" }
            );

            modelBuilder.Entity<Room>()
                .Property(r => r.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
