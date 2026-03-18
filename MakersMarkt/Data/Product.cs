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
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Material { get; set; }
        public int ProductionTime { get; set; }
        public string Complexity { get; set; }
        public string Sustainability { get; set; }
        public string UniqueFeatures { get; set; }
        public decimal Price { get; set; }

        public int MakerId { get; set; }
        public User Maker { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<ProductImage> Images { get; set; }
    }
}
