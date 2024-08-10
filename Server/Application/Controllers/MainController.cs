using BuisnesLogic.ServicesInterface;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Application.Models;
using ApplicationInfrastructure;
using Application.Models.ViewModels;
using DomainModel.Entity;
using BuisnesLogic.Extensions;
using AutoMapper;
using Contracts;
using System.Text.Json;
using BuisnesLogic.ConstStorage;
using Microsoft.AspNetCore.Identity;
using DataBase.AppDbContexts;
using Microsoft.EntityFrameworkCore;
namespace Application.Controllers
{

    public class MainController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Program> _logger;
        private readonly MainDbContext _mainDbContext;
        private readonly UserManager<User> _usermanager;
        public MainController(IConfiguration configuration, ILogger<Program> logger, MainDbContext mainDbContext, UserManager<User> userManager)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mainDbContext = mainDbContext ?? throw new ArgumentNullException(nameof(mainDbContext)); 
            _usermanager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
        public IActionResult Catalog()
        {
            using (var httpclient = new HttpClient())
            {
                // ProductEndpoint
                string endpoint = Url.Action("GetAllProducts", "ProductEndpoint")!;
                string url = _configuration.GetServerAddres() + endpoint;
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
        public async Task<IActionResult> ProductsInCatalog(int id)
        {
            using (var httpclient = new HttpClient())
            {
                string endpoint = Url.Action("GetProductById", "ProductEndpoint", new {id})!;
                string url = _configuration.GetServerAddres() + endpoint;
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
        public async Task<IActionResult> FindProduct([FromForm] string name)
        {
            using(var httpclient = new HttpClient())
            {
                string endpoint = _configuration.GetServerAddres() + Url.Action(EndpointValueInStringStorage.ProductByNameAction, EndpointValueInStringStorage.ProductControllerWithWebServer, new {name});
                var resp = await httpclient.GetAsync(endpoint);
                switch(resp.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound: return View(EndpointValueInStringStorage.CatalogAction, new List<Product>());
                    case System.Net.HttpStatusCode.BadRequest: return View(EndpointValueInStringStorage.CatalogAction, new List<Product>());
                }
                if (!resp.IsSuccessStatusCode) return BadRequest();
                var model = await resp.Content.ReadFromJsonAsync<IEnumerable<Product>>();
                return View(EndpointValueInStringStorage.CatalogAction,model);
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost("/Main/Add/{id:int}/{email}")]
        [Authorize]
        [ActionName("Add")]
        public async Task<IActionResult>AddProductInBucket([FromRoute] int id, string email)
        {
          
            using(var httpclient = new HttpClient())
            {
                string endpoint = Url.Action("GetProductById", "ProductEndpoint", new { id })!;
                string url = _configuration.GetServerAddres() + endpoint;
                Product? product = null;
                try
                {
                    product = await httpclient.GetFromJsonAsync<Product>(url);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + this.ToString());
                }
               if (product is null) { return BadRequest(); }

                var productInBucket = new ProductInBucket()
                {
                    Name = product.Name,
                    OldPrice = product.OldPrice,
                    Count = 1,
                    Quantity = product.Quantity,
                    Url = product.Url,
                    Id = new Guid()
                };
                ProductInBucketContract contract = new ProductInBucketContract()
                {
                    UserEmail = email,
                    Product = productInBucket
                };
                var requestUrl = _configuration.GetServerAddres() + Url.Action("AddInBucket", "BucketOperationendEndPoints");
                var res = await httpclient.PostAsJsonAsync(requestUrl, contract);
                if (!res.IsSuccessStatusCode) return BadRequest();
                return RedirectToAction(EndpointValueInStringStorage.CatalogAction, EndpointValueInStringStorage.MainController);
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult Bucket([FromQuery] string email)
        {
            try
            {
                var user = _mainDbContext.Users.Include(prs => prs.ProductsInBucket).AsEnumerable().Where(u => u.Email == email).SingleOrDefault();
                if (user is null) return BadRequest();
                return View(user.ProductsInBucket);
            }
            catch 
            {
                return BadRequest();
            }
        }
        [HttpPost]
        [ActionName("Pay")]
        public IActionResult YandexMoneyNotification(PayModel model)
        {
            if(model is null)
            {
                _logger.LogError(string.Concat(DateTime.Now, " ", nameof(YandexMoneyNotification), " ", "model was null"));
                return BadRequest();
            }
            _logger.LogInformation(string.Concat(model.datetime.ToString(), ":", model.withdraw_amount, " ", model.notification_type));
            return Ok();

        }
    }
}
