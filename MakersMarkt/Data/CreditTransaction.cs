using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakersMarkt.Data
{
    public class CreditTransaction
    {
        public int Id { get; set; }

        public int FromUserId { get; set; }
        public int ToUserId { get; set; }

        public decimal Amount { get; set; }
        public string Type { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
