using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakersMarkt.Data
{
    public class Order
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public User Buyer { get; set; }

        public decimal TotalPrice { get; set; }
        public string Status { get; set; }

        public List<OrderItem> Items { get; set; }
        public List<OrderStatusHistory> History { get; set; }
    }
}
