using Microsoft.AspNetCore.Mvc;
using WebClient.Models;
using WebServer.AppDbContexts;

namespace WebClient.Controllers
{
    public class AdminController : Controller
    {
        public AdminController() { }
        [HttpGet]
        public IActionResult AddProductGet() => View();

        [HttpPost]
        public IActionResult AddProductPost([FromForm] ProductViewModel productViewModel)
        {

        }
    }
}
