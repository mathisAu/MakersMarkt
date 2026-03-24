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
                  Username = "gebruiker1",
                  PasswordHash = BCrypt.Net.BCrypt.HashPassword("gebruiker123"),
                  Role = "gebruiker",
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
            // Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Handgemaakte zilveren ring",
                    Description = "Zilveren ring met minimalistisch design.",
                    Type = "Ring",
                    Material = "Zilver",
                    ProductionTime = 7,             // dagen
                    Complexity = "Gemiddeld",
                    Sustainability = "Gerecycled zilver",
                    UniqueFeatures = "Handgegraveerd patroon",
                    Price = 59.99m,
                    Image = "StoreLogo.png",      // wordt Images/silver_ring.png via ImagePath
                    MakerId = 1,                    // verwijst naar User Id 1 (moderator)
                    CategoryId = 1                  // verwijst naar Category Id 1 (Sieraden)
                },
                new Product
                {
                    Id = 2,
                    Name = "Keramische vaas",
                    Description = "Handgedraaide keramische vaas met glazuurafwerking.",
                    Type = "Vaas",
                    Material = "Keramiek",
                    ProductionTime = 14,
                    Complexity = "Hoog",
                    Sustainability = "Lokaal geproduceerde klei",
                    UniqueFeatures = "Unieke glazuurpatronen",
                    Price = 89.50m,
                    Image = "StoreLogo.png",
                    MakerId = 2,                    // verwijst naar gebruiker1
                    CategoryId = 2                  // Keramiek
                },
                new Product
                {
                    Id = 3,
                    Name = "Geweven wollen sjaal",
                    Description = "Zachte handgeweven sjaal van merinowol.",
                    Type = "Sjaal",
                    Material = "Wol",
                    ProductionTime = 5,
                    Complexity = "Laag",
                    Sustainability = "Biologische wol",
                    UniqueFeatures = "Uniek kleurverlooppatroon",
                    Price = 39.95m,
                    Image = "StoreLogo.png",
                    MakerId = 2,
                    CategoryId = 3                  // Text
                }
            );
        }
    }
}