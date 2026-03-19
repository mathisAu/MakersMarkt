using System;
using System.Collections.Generic;

namespace MakersMarkt.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = null!;
        public decimal Credit { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? Description { get; set; }
        public string? ProfileImageUrl { get; set; }

        public List<Product> Products { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public List<CreditTransaction> SentTransactions { get; set; } = new();
        public List<CreditTransaction> ReceivedTransactions { get; set; } = new();
    }
}