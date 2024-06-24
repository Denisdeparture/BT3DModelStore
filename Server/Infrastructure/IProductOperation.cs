using DomainModel;
using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IProductOperation
    {
        public Task<string> CreateProductAsync(Product product, Stream model, string nameofobjectinstorage, string? pathfromstorage = null);
        public Task<string> UpdateProductAsync(Product product, Stream model, string nameofobjectinstorage, string? pathfromstorage = null);
        public Task<bool> DeleteProductAsync(string url, string nameofobjectinstorage, string? pathfromstorage = null);
        public Task<bool> DeleteProductAsync(Product product, string nameofobjectinstorage, string? pathfromstorage = null);
        public IEnumerable<Product> GetAllProducts();
    }
}
