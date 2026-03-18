using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakersMarkt.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // maker, buyer, moderator
        public decimal Credit { get; set; }

        public List<Product> Products { get; set; }
        public List<Order> Orders { get; set; }
    }
}
