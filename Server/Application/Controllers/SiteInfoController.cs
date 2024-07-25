using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
{
    public class SiteInfoController : Controller
    {
       
        public IActionResult Contacts() => View();
      
        public IActionResult Payment() => View();
       
        public IActionResult Delivery() => View();

        public IActionResult About() => View();
    }
}
