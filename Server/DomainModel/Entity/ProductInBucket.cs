using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Entity
{
    public class ProductInBucket 
    {
        public int Count { get; set; }
        public string? ProductId { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public double Price { get; set; }
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        public int Quantity { get; set; }
        public double? OldPrice { get; set; }
        public User User { get; set; } = null!;
    }
}
