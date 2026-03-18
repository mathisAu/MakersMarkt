using System;
using System.Collections.Generic;

namespace MakersMarkt.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }  
        public string Role { get; set; } // maker, koper, moderator
        public decimal Credit { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Product> Products { get; set; }
        public List<Order> Orders { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<Review> Reviews { get; set; }
        public List<CreditTransaction> SentTransactions { get; set; }
        public List<CreditTransaction> ReceivedTransactions { get; set; }
    }
}