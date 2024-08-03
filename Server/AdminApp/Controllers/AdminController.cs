using BuisnesLogic.ServicesInterface;
using Infrastructure;
using DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BuisnesLogic.ConstStorage;
using Contracts;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using BuisnesLogic.Model.Roles;
namespace Application.Controllers
{
    public partial class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<User> _userManager;
        public AdminController(IConfiguration configuration, ILogger<AdminController> logger, UserManager<User> userManager)
        {
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
        }
        [HttpGet("/Admin/AdminPanel")]
        public async Task<IActionResult> AdminPanel([FromQuery] string email)
        {
            var action = Url.Action(EndpointValueInStringStorage.UserInfoActionInWebServer, EndpointValueInStringStorage.AccountControllerWithWebServer, new { email });
            var baseuri = string.Format(string.Format("{0}://{1}", _configuration["ServerPath:Protocol"]!, _configuration["ServerPath:Host"]));
            User? dataUser = null;
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var res = await httpClient.GetAsync(baseuri + action);
                    switch (res.StatusCode)
                    {
                        case HttpStatusCode.NotFound: return NotFound();
                        case HttpStatusCode.BadRequest: return BadRequest();
                    }
                    var jsonResponse = await res.Content.ReadAsStringAsync();
                    dataUser = JsonSerializer.Deserialize<User>(jsonResponse);

                }
                catch (Exception ex)
                {
                    _logger.LogError(DateTime.UtcNow + Environment.NewLine + ex.Message + Environment.NewLine + this.ToString());
                }
            }
            if (dataUser is null)
            {
                string error = $"{this.ToString()} in {nameof(AdminPanel)} user was null";
                _logger.LogError(error);
                return BadRequest();
            }
            var roles = await _userManager.GetRolesAsync(dataUser) ?? throw new NullReferenceException();
            var isAdmin = roles.FirstOrDefault(r => r == RolesType.Admin.ToString());
            if(string.IsNullOrEmpty(isAdmin)) return Unauthorized();
            return View();
        }
       
    }
    
}
