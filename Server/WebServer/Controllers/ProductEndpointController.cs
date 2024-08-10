using WebServer;
using Microsoft.AspNetCore.Mvc;
using DataBase.AppDbContexts;
using Infrastructure;
using WebServer.RealizationInterface;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace WebServer.Controllers
{
    public class ProductEndpointController : ControllerBase
    {
        private readonly ILogger<Program> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IProductOperation _productOperation;

        public ProductEndpointController( IServiceProvider serviceProvider,IProductOperation productOperation, IConfiguration configuration, ILogger<Program> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(_serviceProvider));
            _productOperation = productOperation ?? throw new ArgumentNullException(nameof(productOperation));
        }
        [HttpGet("/ProductEndpoint/GetAllProducts")]
        public IResult GetAllProducts()
        {
            var resp = _productOperation.GetAllProducts();
            if (resp is null)
            {
                return Results.NoContent();
            }
            return Results.Json(resp);
        }
        [HttpGet("/ProductEndpoint/GetProductById")]
        public IResult GetProductById([FromQuery] int id)
        {
            if (id <= -1) return Results.BadRequest();
            var product = _productOperation.GetById(id);
            if(product is null) return Results.NotFound();
            return Results.Json(product);
        }
        [HttpGet("/ProductEndpoint/GetProductByName")]
        public IResult GetProductByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return Results.BadRequest();
            var product = _productOperation.GetByName(name);
            if (product is null || product.ToList().Count == 0) return Results.NotFound();
            return Results.Json(product);
        }
    }
}
