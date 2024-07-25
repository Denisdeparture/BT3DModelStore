using BuisnesLogic.ServicesInterface;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Application.Models;
using DomainModel;
using ApplicationInfrastructure;
using Application.Models.ViewModels;
namespace Application.Controllers
{

    public class MainController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Program> _logger;
        public MainController(IConfiguration configuration, ILogger<Program> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public IActionResult Catalog()
        {
            using (var httpclient = new HttpClient())
            {
                // ProductEndpoint
                string endpoint = Url.Action("GetAllProducts", "ProductEndpoint")!;
                string url = _configuration["ServerPath:Protocol"] + "://" + _configuration["ServerPath:Host"] + endpoint;
                _logger.LogInformation(url);
                var task = httpclient.GetFromJsonAsync<IEnumerable<Product>>(url);
                try
                {
                    task.Wait();
                    var allProducts = task.Result;
                    return View(allProducts);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + this.ToString());
                    return BadRequest();
                }

            }
        }
        [Route("/Catalog/{id:int}")]
        public async Task<IActionResult> ProductInCatalog(int id)
        {
            using (var httpclient = new HttpClient())
            {
                string endpoint = Url.Action("GetProductById", "ProductEndpoint", new {id})!;
                string url = _configuration["ServerPath:Protocol"] + "://" + _configuration["ServerPath:Host"] + endpoint;
                Product? product = null;
                try
                {
                    product = await httpclient.GetFromJsonAsync<Product>(url);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + this.ToString());
                }
                if(product is null) { return BadRequest(); }
                return View(product);

            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
