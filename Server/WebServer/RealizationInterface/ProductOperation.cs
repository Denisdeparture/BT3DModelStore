using BuisnesLogic.Model;
using Infrastructure;
using DataBase.AppDbContexts;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using DomainModel.Entity;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;

namespace WebServer.RealizationInterface
{
    public class ProductOperation : IProductOperation
    {
        private readonly MainDbContext _database;
        private readonly IYandexClient _yandexClient;
        private readonly IConfiguration _configuration;
        public ProductOperation(MainDbContext database, IYandexClient yandexcloudoperation, IConfiguration configuration)
        {
            _database = database;
            _yandexClient = yandexcloudoperation;
            _configuration = configuration;
        }
        public async Task<string> CreateProductAsync(Product product, Stream model, string nameofobjectinstorage)
        {
            var res = await _yandexClient.AddModel(model,  _configuration["PathFromStorage"]!, nameofobjectinstorage);
            if (!res.isCorrect) throw new Exception();
            product.Url = res.resultUrlFromModel!;
            _database.Products.Add(product);
            _database.SaveChanges();
            return res.resultUrlFromModel!;
        }

        public async Task<bool> DeleteProductAsync(Product product, string nameofobjectinstorage)
        {
            var res = await _yandexClient.DeleteModel(_configuration["PathFromStorage"]!, nameofobjectinstorage);
            if (res) throw new Exception();
            var prod = _database.Products.Where(p =>
            p.Name == product.Name &
            p.Price == product.Price &
            p.Description == product.Description).SingleOrDefault();
            if (prod == null) throw new NullReferenceException(nameof(prod));
            _database.Remove(prod);
            _database.SaveChanges();
            return true;

        }
        public async Task<bool> DeleteProductAsync(string url, string nameofobjectinstorage)
        {
            var prod = _database.Products.Where(p =>
            p.Url == url).SingleOrDefault();
            if (prod is null) throw new NullReferenceException(nameof(prod));
            var res = await _yandexClient.DeleteModel(_configuration["PathFromStorage"]!, nameofobjectinstorage);
            if (res) throw new Exception();
            _database.Remove(prod);
            _database.SaveChanges();
            return true;
        }

        public async Task DeleteProductAsync(int id, string? nameofobjectinstoragel = null)
        {
            var prod = GetById(id);
            if (prod is null) throw new NullReferenceException(nameof(prod));
            if(!string.IsNullOrWhiteSpace(nameofobjectinstoragel))
            {
                var res = await _yandexClient.DeleteModel(_configuration["PathFromStorage"]!, nameofobjectinstoragel);
                if (res) throw new Exception(" Delete model was incorrect " + this.ToString());
            }
            _database.Remove(prod);
            _database.SaveChanges();
        }

        public IEnumerable<Product> GetAllProducts() => _database.Products;

        public Product? GetById(int id)
        {
            var allProducts = GetAllProducts();
            return allProducts.Where(pr => pr.Id == id).SingleOrDefault();
        }
        public IEnumerable<Product> GetByName(string name)
        {
            var allproducts = GetAllProducts();
            return allproducts.Where(pr => pr.Name.Equals(name));
        }
        public async Task<string> UpdateProductAsync(Product product, Stream model, string nameofobjectinstorage)
        {
            var res = await _yandexClient.UpdateModel(model, _configuration["PathFromStorage"]!, nameofobjectinstorage);
            if (!res.isCorrect) throw new Exception();
            _database.Update(product);
            _database.SaveChanges();
            return res.resultUrlFromModel!;
        }
    } 
    
}
