using AppServiceInterfice.ServicesInterface;
using AutoMapper;
using DomainInfrastructure;
using DomainModel;
using WebClient.Models;
using WebServer.AppDbContexts;

namespace WebClient.RealizationInterface
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
        public async Task<bool> CreateProduct(ProductViewModel productview)
        {
            var product = Mapping(productview);
            var res = await _yandexClient.AddModel(productview.Model.OpenReadStream(), _configuration["PathFromStorage"]!, productview.Model.FileName);
            if (!res.isCorrect) throw new Exception();
            product.Url = res.resultUrlFromModel!;
            _database.Products.Add(product);
        }

        public void DeleteProduct(ProductViewModel productview)
        {
            var product = Mapping(productview);
            

        }

        public IEnumerable<Product> GetAllProduct() => _database.Products;
        

        public void UpdateProduct(int id)
        {
            throw new NotImplementedException();
        }
        private Product Mapping(ProductViewModel viewmodel)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<ProductViewModel, Product>()
                 .ForMember("Quantity", opt => opt.MapFrom(o => o.Quantity))
                 .ForMember("Price", opt => opt.MapFrom(o => o.Price))
                 .ForMember("Name", opt => opt.MapFrom(o => o.Name))));
            var product = mapper.Map<Product>(viewmodel);
            return product;
        }
    }
}
