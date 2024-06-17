using AutoMapper;
using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Diagnostics;
using System.Security.Claims;
using WebClient.Models;
using System.Runtime.ConstrainedExecution;
using WebServer.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using ApplicationInfrastructure;
namespace WebClient.Controllers
{
    public partial class AccountController : Controller
    {
        private readonly ILogger<Program> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(ILogger<Program> logger, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _logger = logger;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        [HttpGet]
        [ActionName("Login")]
        public async Task<IActionResult> LoginGet(string returnurl)
        {
            return View(new LoginViewModel() { genericDatas = await _signInManager.GetExternalAuthenticationSchemesAsync(), ReturnUrl = returnurl });
        }
        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> LoginPost([FromForm] LoginViewModel loginmodel)
        {
            if (!ModelState.IsValid) return View(loginmodel);
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<LoginViewModel, User>()
                  .ForMember("Email", opt => opt.MapFrom(o => o.Mail))
                  .ForMember("PasswordHash", opt => opt.MapFrom(src => src.Password))));
            var usermap = mapper.Map<User>(loginmodel);
            using (HttpClient client = new HttpClient())
            {
                var resp = await GetResponsed(client, "Login", usermap);
                switch (resp.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        {
                            return Unauthorized();
                        }
                }
                var info = new JwtSecurityTokenHandler().ReadJwtToken(await resp.Content.ReadAsStringAsync());
                await _signInManager.SignInWithClaimsAsync(usermap, new AuthenticationProperties()
                {
                    ExpiresUtc = info.ValidTo
                }, info.Claims);
                
                var actionUrlFormRedirect = Url.Action("Katalog", "Main");
                return Redirect(actionUrlFormRedirect!);
            }

        }
        public IActionResult ProviderAuthentication(string provider, string returnurl)
        {
            // здесь что-то в духе рефлексии 3 аргумент это аргумент передаваемый в функцию
            var actionUrlFormRedirect = Url.Action(nameof(ProviderAuthenticationCallback), "Account", new { returnurl });
            var prop = _signInManager.ConfigureExternalAuthenticationProperties(provider, actionUrlFormRedirect);

            return Challenge(prop, provider);
        }
        public async Task<IActionResult> ProviderAuthenticationCallback(string returnUrl)
        {
            ExternalLoginInfo? obj = await _signInManager.GetExternalLoginInfoAsync(returnUrl);
            if (obj == null)
            {
                _logger.LogError("Ошибка в регистрации через стороний сервис");
                return RedirectToAction("Login");
            }
            SignInResult result = await _signInManager.ExternalLoginSignInAsync(obj.LoginProvider, obj.ProviderKey, false, false);
            if (result.Succeeded)
            {
                var actionUrlFormRedirect = Url.Action("Katalog", "Main");
                return Redirect(actionUrlFormRedirect!);
            }
            return RedirectToAction("Registration", new RegisterViewModel() { Mail = (obj.Principal.FindFirstValue(ClaimTypes.Email) ?? obj.Principal.FindFirstValue(ClaimTypes.Name))!, Password = obj.ProviderKey });
        }
        [HttpGet]
        [ActionName("Registration")]
        public async Task<IActionResult> RegistrationGet([FromQuery] RegisterViewModel registermodel)
        {
            if (!ModelState.IsValid) return View(registermodel);
            if (registermodel.Mail != null & registermodel.Password != null)
            {
                registermodel.Name = (string.IsNullOrEmpty(registermodel.Name)) ? registermodel.Mail : registermodel.Name;
                return await Register(registermodel, "Katalog");
            }
            return View();
        }
        [HttpPost]
        [ActionName("Registration")]
        public async Task<IActionResult> RegistrationPost([FromForm] RegisterViewModel registermodel)
        {
            if (!ModelState.IsValid)
            {
                return View(registermodel);
            }
            return await Register(registermodel, "Katalog");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            var actionUrlFormRedirect = Url.Action("Login", "Account");
            return Redirect(actionUrlFormRedirect!);
        }
       
    }
    public partial class AccountController : Controller
    {
        private async Task<HttpResponseMessage> GetResponsed(HttpClient client, string Action, User data)
        {
            var baseuri = string.Format(string.Format("{0}://{1}", _configuration["ServerPath:Protocol"]!, _configuration["ServerPath:Host"]));
            var path = baseuri + Url.Action(Action, "AccountEndpoint", new {data.Email, data.PasswordHash, data.UserName});
            var resp = await client.GetAsync(path);
            return resp;
        }
        private async Task<IActionResult> Register(RegisterViewModel registermodel, string mainexitaction)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<RegisterViewModel, User>()
                    .ForMember("Email", opt => opt.MapFrom(o => o.Mail))
                    .ForMember("PasswordHash", opt => opt.MapFrom(src => src.Password))
                    .ForMember("UserName", opt => opt.MapFrom(src => src.Name + src.LastName ?? string.Empty))));
            var usermap = mapper.Map<User>(registermodel);
            using (var client = new HttpClient())
            {
                var resp = await GetResponsed(client, "Registration", usermap);
                switch (resp.StatusCode)
                {
                    case System.Net.HttpStatusCode.Conflict:
                        {
                            _logger.LogInformation("User just Register");
                            return Conflict();
                        }
                    case System.Net.HttpStatusCode.BadRequest:
                        {
                            _logger.LogError("Bag Server in Register");
                            return BadRequest();
                        }
                }
                var token = await resp.Content.ReadAsStringAsync();
                var info = new JwtSecurityTokenHandler().ReadJwtToken(token);
                await _signInManager.SignInWithClaimsAsync(usermap, new AuthenticationProperties()
                {
                    ExpiresUtc = info.ValidTo
                }, info.Claims);
                var redirectUrl = Url.Action(mainexitaction, "Main");
                return Redirect(redirectUrl);

            }
        }
    }
}

