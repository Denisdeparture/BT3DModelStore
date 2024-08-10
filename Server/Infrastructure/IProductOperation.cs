using DomainModel.Entity;
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
        public Task<string> CreateProductAsync(Product product, Stream model, string nameofobjectinstorage);
        public Task<string> UpdateProductAsync(Product product, Stream model, string nameofobjectinstoragel);
        public Task<bool> DeleteProductAsync(string url, string nameofobjectinstorage);
        public Task<bool> DeleteProductAsync(Product product, string nameofobjectinstoragel);
        public Task DeleteProductAsync(int id, string? nameofobjectinstoragel = null);
        public Product? GetById(int id);
        public IEnumerable<Product> GetAllProducts();
        public IEnumerable<Product> GetByName(string name);
    }
}
