using BuisnesLogic.ServicesInterface;
using AutoMapper;
using Infrastructure;
using DomainModel;
using Microsoft.AspNetCore.Mvc;
using Application.Models;
using Application.RealizationInterface;
using DataBase.AppDbContexts;
using Application.Filters;
namespace Application.Controllers
{
    public partial class AdminController : Controller
    {
        private readonly IProductOperation _productManager;
        public AdminController(MainDbContext database, IYandexClient client, IConfiguration configuration)
        {
            _productManager = new ProductOperation(database, client, configuration);
        }
        [ActionName("AddProduct")]
        [HttpGet]
        public IActionResult AddProductGet() => View();
        [NullArgumentFilter]
        [ActionName("AddProduct")]
        [HttpPost]
        public async Task<IActionResult> AddProductPost([FromForm] ProductViewModel? productViewModel)
        {
            var product = Mapping(productViewModel);
            var isExist = (_productManager.GetAllProducts().Where(prod =>
            prod.Name == product.Name &
            prod.Description == product.Description
            ).SingleOrDefault()) is not null;
            if (isExist) return BadRequest();
            try
            {
                await _productManager.CreateProductAsync(product, productViewModel.Model.OpenReadStream(), productViewModel.Model.FileName);
            }
            catch (Exception ex)
            {
                throw new DivideByZeroException();
            }
            var redUrl = Url.Action("Katalog", "Main");
            return Redirect(redUrl!);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductViewModel productViewModel)
        {
            var product = Mapping(productViewModel);
            var productInBase = (_productManager.GetAllProducts().Where(prod =>
            prod.Name == product.Name &
            prod.Description == product.Description
            ).SingleOrDefault());
            if(productInBase != null)await _productManager.UpdateProductAsync(productInBase, productViewModel.Model.OpenReadStream(), productViewModel.Model.FileName);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(string url, string namefile)
        {
            if (string.IsNullOrEmpty(url) & string.IsNullOrEmpty(namefile)) return BadRequest();
            var res = await _productManager.DeleteProductAsync(url, namefile);
            if (!res) return BadRequest();
            return Ok();
        }
    }
    public partial class AdminController
    {
        private Product Mapping(ProductViewModel productViewModel)
        {
            try
            {
                var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<ProductViewModel, Product>()
                .ForMember("Name", opt => opt.MapFrom(o => o.Name))
                .ForMember("Price", opt => opt.MapFrom(o => o.Price))
                .ForMember("Quantity", opt => opt.MapFrom(o => o.Quantity))
                .ForMember("Description", opt => opt.MapFrom(o => o.Description))));
                var product = mapper.Map<Product>(productViewModel);
                if (product == null) throw new Exception();
                return product;
            }catch(Exception ex)
            {
                throw new ArgumentException();
            }
        }
    }
}
