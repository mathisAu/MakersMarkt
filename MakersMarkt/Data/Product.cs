using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakersMarkt.Data
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Material { get; set; } = null!;
        public int ProductionTime { get; set; }
        public string Complexity { get; set; } = null!;
        public string Sustainability { get; set; } = null!;
        public string UniqueFeatures { get; set; } = null!;
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public int MakerId { get; set; }
        public User Maker { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
