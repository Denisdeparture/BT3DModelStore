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
            _configuration = configuration;
            _logger = logger;
        }
        [Authorize]
        public IActionResult Katalog()
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
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
