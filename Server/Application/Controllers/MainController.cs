using BuisnesLogic.ServicesInterface;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Application.Models;
using Application.RealizationInterface;
using DataBase.AppDbContexts;
using ApplicationInfrastructure;
namespace Application.Controllers
{

    public class MainController : Controller
    {
        private readonly IProductOperation _productManager;
        private readonly ILogger<Program> _logger;
        public MainController(MainDbContext database, IYandexClient client, IConfiguration configuration, ILogger<Program> logger)
        {
            _productManager = new ProductOperation(database, client, configuration);
            _logger = logger;
        }
        [Authorize]
        public IActionResult Katalog()
        {
            return View(_productManager.GetAllProducts());
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
