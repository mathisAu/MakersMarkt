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
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<CreditTransaction> CreditTransactions { get; set; }

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
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new User
                {
                    Id = 2,
                    Username = "maker1",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("maker123"),
                    Role = "maker",
                    Credit = 0,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new User
                {
                    Id = 3,
                    Username = "koper1",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("koper123"),
                    Role = "koper",
                    Credit = 100,
                    CreatedAt = new DateTime(2026, 1, 1)
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