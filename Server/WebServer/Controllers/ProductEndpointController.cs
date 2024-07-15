using WebServer;
using Microsoft.AspNetCore.Mvc;
using DataBase.AppDbContexts;
using Infrastructure;
using WebServer.RealizationInterface;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
namespace WebServer.Controllers
{
    public class ProductEndpointController : ControllerBase
    {
        private readonly ILogger<Program> _logger;
        private readonly IProductOperation _productManager;
        public ProductEndpointController(MainDbContext database, IYandexClient client, IConfiguration configuration, ILogger<Program> logger)
        {
            _productManager = new ProductOperation(database, client, configuration);
            _logger = logger;
        }
        [HttpGet("/ProductEndpoint/GetAllProducts")]
        public IResult GetAllProducts()
        {
            var resp = _productManager.GetAllProducts();
            if(resp is null)
            {
                return Results.NoContent();
            }
           return Results.Json(resp);
           
            
            //    _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + this.ToString() + Environment.NewLine + DateTime.UtcNow);
            //    return Results.BadRequest();
            //}
        }
    }
}
