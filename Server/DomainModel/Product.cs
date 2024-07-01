using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    public class Product
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Url { get; set; } = null!;
    }
}
