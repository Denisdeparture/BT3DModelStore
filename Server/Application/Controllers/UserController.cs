using Application.Models.ViewModels;
using BuisnesLogic.ConstStorage;
using DomainModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace Application.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<User> _userManager;
        public UserController(IConfiguration configuration, ILogger<UserController> logger, UserManager<User> userManager)
        {
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
        }
        [Authorize]
        [HttpGet("/User/UserAccountPanel/{email}")]
        public async Task<IActionResult> UserAccountPanel(string email)
        {
            var action = Url.Action(EndpointValueInStringStorage.UserInfoActionInWebServer, EndpointValueInStringStorage.AccountControllerWithWebServer, new {email});
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
                    var jsonResponse =  await res.Content.ReadAsStringAsync();
                    dataUser = JsonSerializer.Deserialize<User>(jsonResponse);

                }
                catch (Exception ex)
                {
                    _logger.LogError(DateTime.UtcNow + Environment.NewLine + ex.Message + Environment.NewLine + this.ToString());
                }
            }
            if (dataUser is null)
            {
                _logger.LogError($"{this.ToString()} in {nameof(UserAccountPanel)} user was null");
                throw new NullReferenceException($"{this.ToString()} in {nameof(UserAccountPanel)} user was null");
            }
            var roles = await _userManager.GetRolesAsync(dataUser);
            var user = new AccountViewModel()
            {
                About = dataUser,
                Roles = roles
                
            };
            return View(EndpointValueInStringStorage.UserPanelAction,user);
        }
    }
}
