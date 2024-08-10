using DomainModel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Entity
{
    public class PurchasedProduct : IProductStatus
    {
        public Product? Product { get; set; }
        public int Id { get; set; }
        public int Status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int PayStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
