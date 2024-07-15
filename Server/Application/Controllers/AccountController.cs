using AutoMapper;
using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Diagnostics;
using System.Security.Claims;
using System.Runtime.ConstrainedExecution;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using ApplicationInfrastructure;
using System.Net;
using Application.Models.ViewModels;
using BuisnesLogic.ServicesInterface;
using BuisnesLogic.ServicesInterface.ClientsInterfaces.MessageBrokers;
using BuisnesLogic.Service.Managers;
using Newtonsoft.Json.Linq;
using BuisnesLogic.Service;
using System.Text.Json;
using Contracts;
namespace Application.Controllers
{
    public partial class AccountController : Controller
    {
        private readonly ILogger<Program> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IStrategyValidation _validator;
        private readonly IProduceClient<User> _messageBroker;
        private readonly JwtManager _jwtManager;
        public AccountController(ILogger<Program> logger, SignInManager<User> signInManager, IConfiguration configuration, IStrategyValidation validation, IMessageBrokerClient<User> messageBroker, JwtManager jwtManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _configuration = configuration;
            _validator = validation;
            _messageBroker = messageBroker;
            _jwtManager = jwtManager;
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
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<LoginViewModel, User>()
                  .ForMember("Email", opt => opt.MapFrom(o => o.Mail))
                  .ForMember("PasswordHash", opt => opt.MapFrom(src => src.Password))));
            var usermap = mapper.Map<User>(loginmodel);
            using (HttpClient client = new HttpClient())
            {
                var resp = await GetResponsed(client, "Login", usermap);
                switch (resp.StatusCode)
                {
                    case HttpStatusCode.Unauthorized: return Unauthorized();
                }
                var userAndJwt = JsonSerializer.Deserialize<UserJwtContract>(await resp.Content.ReadAsStringAsync());
                var info = new JwtSecurityTokenHandler().ReadJwtToken(userAndJwt!.JwtToken);
                await _signInManager.SignInWithClaimsAsync(userAndJwt.User, new AuthenticationProperties()
                {
                    ExpiresUtc = info.ValidTo
                }, info.Claims);
                return Redirect(Url.Action("Katalog", "Main")!);
            }
        }
        public IActionResult ProviderAuthentication(string provider, string returnurl)
        {
            var actionUrlFormRedirect = Url.Action(nameof(ProviderAuthenticationCallback), "Account", new { returnurl });
            var prop = _signInManager.ConfigureExternalAuthenticationProperties(provider, actionUrlFormRedirect);
            return Challenge(prop, provider);
        }
        public async Task<IActionResult> ProviderAuthenticationCallback(string returnUrl)
        {
            ExternalLoginInfo? obj = await _signInManager.GetExternalLoginInfoAsync(returnUrl);
            if (obj is null)
            {
                _logger.LogError("Ошибка в регистрации через стороний сервис");
                return RedirectToAction("Login");
            }
            SignInResult result = await _signInManager.ExternalLoginSignInAsync(obj.LoginProvider, obj.ProviderKey, false, false);
            if (result.Succeeded) return Redirect(Url.Action("Katalog", "Main")!);
            return RedirectToAction("Registration", new RegisterViewModel() { Mail = (obj.Principal.FindFirstValue(ClaimTypes.Email) ?? obj.Principal.FindFirstValue(ClaimTypes.Name))!, Password = obj.ProviderKey, PasswordConfirm = obj.ProviderKey });
        }
        [HttpGet]
        [ActionName("Registration")]
        public async Task<IActionResult> RegistrationGet([FromQuery] RegisterViewModel registermodel)
        {
            if (registermodel.Mail != null & registermodel.Password != null)
            {
                registermodel.Name = (string.IsNullOrEmpty(registermodel.Name)) ? registermodel.Mail : registermodel.Name;
                return await RegisterFromMessageBroker(registermodel, "Katalog");
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
            return await RegisterFromMessageBroker(registermodel, "Katalog");
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
            //client.
            var resp = await client.GetAsync(path);
            return resp;
        }
        private async Task<IActionResult> Register(RegisterViewModel registermodel, string mainexitaction)
        {
            var usermap = MappingRegisterModelFromUser(registermodel);
            using (var client = new HttpClient())
            {
                var resp = await GetResponsed(client, "Registration", usermap);
                var token = await resp.Content.ReadAsStringAsync();
                if (resp.StatusCode != HttpStatusCode.OK | string.IsNullOrEmpty(token))
                {
                    return BadRequest();
                }
                var info = new JwtSecurityTokenHandler().ReadJwtToken(token);
                await _signInManager.SignInWithClaimsAsync(usermap, new AuthenticationProperties()
                {
                    ExpiresUtc = info.ValidTo
                }, info.Claims);
                var redirectUrl = Url.Action(mainexitaction, "Main");
                return Redirect(redirectUrl);

            }
        }
        private async Task<IActionResult> RegisterFromMessageBroker(RegisterViewModel registermodel, string mainexitaction, string? topic = null)
        {
            var usermap = MappingRegisterModelFromUser(registermodel);
            var cts = new CancellationTokenSource();
            var res = await _messageBroker.Produce(usermap, cts, topic);
            if (!res.Success)
            { 
                _logger.LogError(DateTime.UtcNow + Environment.NewLine + this.ToString() + Environment.NewLine + res.ErrorDescription); 
                return BadRequest();
            }
            var token = await JwtCreator.CreateTokenAsync(usermap, _jwtManager);
            var info = new JwtSecurityTokenHandler().ReadJwtToken(token);
            await _signInManager.SignInWithClaimsAsync(usermap, new AuthenticationProperties()
            {
                ExpiresUtc = info.ValidTo
            }, info.Claims);
            return Redirect(Url.Action(mainexitaction, "Main")!);
        }
        private User MappingRegisterModelFromUser(RegisterViewModel model)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<RegisterViewModel, User>()
                  .ForMember("Email", opt => opt.MapFrom(o => o.Mail))
                  .ForMember("PasswordHash", opt => opt.MapFrom(src => src.Password))
                  .ForMember("UserName", opt => opt.MapFrom(src => src.Name + src.LastName ?? string.Empty))));
            var usermap = mapper.Map<User>(model);
            return usermap;
        }
       
    }
}

