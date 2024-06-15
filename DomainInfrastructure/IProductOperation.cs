using DomainModel;
using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainInfrastructure
{
    public interface IProductOperation
    {
        public bool CreateProduct(Product product);
        public void UpdateProduct(int id);
        public void DeleteProduct(int id);
        public IEnumerable<Product> GetAllProduct();
    }
}
