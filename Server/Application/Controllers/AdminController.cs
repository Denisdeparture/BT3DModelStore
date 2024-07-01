using BuisnesLogic.ServicesInterface;
using AutoMapper;
using Infrastructure;
using DomainModel;
using Microsoft.AspNetCore.Mvc;
using Application.Models;
using Application.RealizationInterface;
using DataBase.AppDbContexts;
using ApplicationInfrastructure;
using Microsoft.IdentityModel.Tokens;
namespace Application.Controllers
{
    public partial class AdminController : Controller
    {
        private readonly IProductOperation _productManager;
        private readonly IStrategyConditions _strategy;
        private readonly ILogger<Program> _logger;
        public AdminController(MainDbContext database, IYandexClient client, IConfiguration configuration, IStrategyConditions cond, ILogger<Program> logger)
        {
            _productManager = new ProductOperation(database, client, configuration);
            _strategy = cond;
            _logger = logger;
        }
        [ActionName("AddProduct")]
        public async Task<IActionResult> AddProductPost([FromForm]ProductViewModel productViewModel)
        {
            var productViewIsValid = _strategy.StrategyCondition(new List<Predicate<ProductViewModel>>()
            {
                (pw) => pw != null,
                (pw) => pw.GetType().GetProperties().Where(pr => pr.GetValue(pw) != null).ToArray().Length == pw.GetType().GetProperties().Length,
                (pw) => pw.GetType().GetProperties().Where(pr => pr.PropertyType.Equals(typeof(String))).Where(pr => string.IsNullOrEmpty((pr.GetValue(pw) as string))).ToArray().Length == pw.GetType().GetProperties().Where(pr => pr.PropertyType.Equals(typeof(String))).ToArray().Length
            }, productViewModel);
            if (!productViewIsValid) return BadRequest();
            var product = Mapping(productViewModel);
            using (var ProductsInDBisValid = _strategy.StrategyCondition(new List<Predicate<IEnumerable<Product>>>()
            {
                (products) => !products.Equals(null),
            }, _productManager.GetAllProducts()))
            {
                if (!ProductsInDBisValid)
                {
                    return NoContent();
                }
                var isExist = (ProductsInDBisValid.Object!.Where(prod =>
                prod.Name == product.Name &
                prod.Description == product.Description
                ).SingleOrDefault()) is not null;
                if (isExist) return BadRequest("this object exist");
            }
            try
            {
                await _productManager.CreateProductAsync(product, productViewModel.Model.OpenReadStream(), productViewModel.Model.FileName);
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError("3Dmodel was null validation isn`t worked " + this.GetType().ToString());
                return Conflict("3Dmodel was null");
            }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductViewModel productViewModel)
        {
            var productViewIsValid = _strategy.StrategyCondition(new List<Predicate<ProductViewModel>>()
            {
                (pw) => pw != null,
                (pw) => pw.GetType().GetProperties().Where(pr => pr.GetValue(pw) != null).ToArray().Length == pw.GetType().GetProperties().Length,
                (pw) => pw.GetType().GetProperties().Where(pr => pr.PropertyType.Equals(typeof(String))).Where(pr => string.IsNullOrEmpty((pr.GetValue(pw) as string))).ToArray().Length == pw.GetType().GetProperties().Where(pr => pr.PropertyType.Equals(typeof(String))).ToArray().Length
            }, productViewModel);
            if (!productViewIsValid) return BadRequest();
            var product = Mapping(productViewModel);
            var productInBase = (_productManager.GetAllProducts().Where(prod =>
            prod.Name == product.Name &
            prod.Description == product.Description
            ).SingleOrDefault());
            if (productInBase == null) return NoContent();
            try
            {
                await _productManager.UpdateProductAsync(productInBase, productViewModel.Model.OpenReadStream(), productViewModel.Model.FileName);
            }
            catch(NullReferenceException ex)
            {
                return Conflict("3Dmodel was null");
            }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(DeleteProductViewModel deleteProductViewModel)
        {
            var deleteViewIsValid = _strategy.StrategyCondition(new List<Predicate<DeleteProductViewModel>>()
            {
                (pw) => string.IsNullOrEmpty(pw.Url),
                (pw) => string.IsNullOrEmpty(pw.NameOfFile)
            }, deleteProductViewModel);
            if (deleteViewIsValid)  return BadRequest();
            var res = await _productManager.DeleteProductAsync(deleteViewIsValid.Object!.Url, deleteViewIsValid.Object!.NameOfFile);
            if (!res) return BadRequest();
            return Ok();
        }
    }
    public partial class AdminController
    {
        private Product Mapping(ProductViewModel productViewModel)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<ProductViewModel, Product>()
                .ForMember("Name", opt => opt.MapFrom(o => o.Name))
                .ForMember("Price", opt => opt.MapFrom(o => o.Price))
                .ForMember("Quantity", opt => opt.MapFrom(o => o.Quantity))
                .ForMember("Description", opt => opt.MapFrom(o => o.Description))));
            var product = mapper.Map<Product>(productViewModel);
            return product;
        }
    }
}
