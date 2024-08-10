using Application.Models.ViewModels;
using BuisnesLogic.ConstStorage;
using BuisnesLogic.Extensions;
using Contracts;
using DomainModel.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace Application.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public UserController(IConfiguration configuration, ILogger<UserController> logger, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [Authorize]
        [HttpGet("/User/UserAccountPanel/{email}")]
        public async Task<IActionResult> UserAccountPanel(string email)
        {
            var action = Url.Action(EndpointValueInStringStorage.UserInfoActionInWebServer, EndpointValueInStringStorage.AccountControllerWithWebServer, new {email});
            var baseuri = _configuration.GetServerAddres();
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
        [HttpPost]
        public async Task<IActionResult> ChangeEmail(string newemail)
        {
            if(string.IsNullOrWhiteSpace(newemail)) return BadRequest();
            using(var httpClient = new HttpClient())
            {

                var contract = new ChangeEmailContract(User.FindFirstValue(ClaimTypes.Email)!, newemail);
                var url = _configuration.GetServerAddres() + Url.Action(EndpointValueInStringStorage.ChangeEmailAction, EndpointValueInStringStorage.AccountControllerWithWebServer);
                var res = await httpClient.PutAsJsonAsync<ChangeEmailContract>(url, contract);
                if (!res.IsSuccessStatusCode) return BadRequest();
                var userAndJwt = await res.Content.ReadFromJsonAsync<UserJwtContract>();
                var info = new JwtSecurityTokenHandler().ReadJwtToken(userAndJwt!.JwtToken);
                await _signInManager.SignOutAsync();
                await _signInManager.SignInWithClaimsAsync(userAndJwt.User, new AuthenticationProperties()
                {
                    ExpiresUtc = info.ValidTo
                }, info.Claims);
                return Redirect(Url.Action(EndpointValueInStringStorage.UserPanelAction,EndpointValueInStringStorage.UserController) + $"/{userAndJwt.User.Email}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ChangePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return BadRequest();
            using(var httpClient = new HttpClient())
            {
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                var contract = new ChangePhoneNumberContract(email, phoneNumber);
                var url = _configuration.GetServerAddres() + Url.Action(EndpointValueInStringStorage.ChangePhoneNumberAction, EndpointValueInStringStorage.AccountControllerWithWebServer);
                var res = await httpClient.PutAsJsonAsync<ChangePhoneNumberContract>(url, contract);
                if (!res.IsSuccessStatusCode) return BadRequest();
                return Redirect(Url.Action(EndpointValueInStringStorage.UserPanelAction, EndpointValueInStringStorage.UserController) + $"/{email}");

            }
        }
    }
}
