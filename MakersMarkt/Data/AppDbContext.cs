using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakersMarkt.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<CreditTransaction> CreditTransactions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=localhost;port=3306;database=makersmarkt;user=root;password=",
                ServerVersion.Parse("8.0.30")
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "moderator",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("mod123"),
                    Role = "moderator",
                    Credit = 9999,
                    CreatedAt = new DateTime(2026, 1, 1),
                    Description = "Moderator van MakersMarkt"
                },
                new User
                {
                    Id = 2,
                    Username = "gebruiker1",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("gebruiker123"),
                    Role = "gebruiker",
                    Credit = 100,
                    CreatedAt = new DateTime(2026, 1, 1),
                    Description = "Liefhebber van handgemaakte spullen"
                }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Sieraden" },
                new Category { Id = 2, Name = "Keramiek" },
                new Category { Id = 3, Name = "Textiel" },
                new Category { Id = 4, Name = "Kunst" }
            );
        }
    }
}