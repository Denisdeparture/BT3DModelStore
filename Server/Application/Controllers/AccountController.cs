using AutoMapper;
using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using ApplicationInfrastructure;
using System.Net;
using Application.Models.ViewModels;
using BuisnesLogic.ServicesInterface;
using BuisnesLogic.ServicesInterface.ClientsInterfaces.MessageBrokers;
using BuisnesLogic.Service.Managers;
using BuisnesLogic.Service;
using System.Text.Json;
using Contracts;
using BuisnesLogic.ConstStorage;
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
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _validator = validation ?? throw new ArgumentNullException(nameof(validation));
            _messageBroker = messageBroker ?? throw new ArgumentNullException(nameof(messageBroker));
            _jwtManager = jwtManager ?? throw new ArgumentNullException(nameof(jwtManager));
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
                var resp = await GetResponsed(client, EndpointValueInStringStorage.LoginAction, usermap);
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
                return Redirect(Url.Action(EndpointValueInStringStorage.CatalogAction, EndpointValueInStringStorage.MainController)!);
            }
        }
        public IActionResult ProviderAuthentication(string provider, string returnurl)
        {
            const string actionForRedirect = nameof(ProviderAuthenticationCallback);
            var actionUrlFormRedirect = Url.Action(actionForRedirect, EndpointValueInStringStorage.AccountContoller, returnurl);
            var prop = _signInManager.ConfigureExternalAuthenticationProperties(provider, actionUrlFormRedirect);
            return Challenge(prop, provider);
        }
        public async Task<IActionResult> ProviderAuthenticationCallback(string returnurl)
        {
            ExternalLoginInfo? loginInfo = await _signInManager.GetExternalLoginInfoAsync(returnurl);
            if (loginInfo is null)
            {
                _logger.LogError("Ошибка в авторизации через стороний сервис");
                return RedirectToAction(EndpointValueInStringStorage.LoginAction);
            }
            SignInResult result = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, false, false);
            if (result.Succeeded) return Redirect(Url.Action(EndpointValueInStringStorage.CatalogAction, EndpointValueInStringStorage.MainController)!);
            return RedirectToAction(EndpointValueInStringStorage.RegistrationAction, new RegisterViewModel() { Mail = (loginInfo.Principal.FindFirstValue(ClaimTypes.Email) ?? loginInfo.Principal.FindFirstValue(ClaimTypes.Name))!, Password = loginInfo.ProviderKey, PasswordConfirm = loginInfo.ProviderKey });
        }
        [HttpGet]
        [ActionName("Registration")]
        public async Task<IActionResult> RegistrationGet([FromQuery] RegisterViewModel registermodel)
        {
            const string nameResultAction = "Catalog";
            const string nameResultController = "Main";
            if (registermodel.Mail != null & registermodel.Password != null)
            {
                registermodel.Name = (string.IsNullOrEmpty(registermodel.Name)) ? registermodel.Mail : registermodel.Name;
                return await RegisterFromMessageBroker(registermodel, (nameResultAction, nameResultController));
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
            return await RegisterFromMessageBroker(registermodel, (EndpointValueInStringStorage.CatalogAction, EndpointValueInStringStorage.MainController));
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            var actionUrlFormRedirect = Url.Action(EndpointValueInStringStorage.LoginAction, EndpointValueInStringStorage.AccountContoller);
            return Redirect(actionUrlFormRedirect!);
        }
       
    }
    public partial class AccountController : Controller
    {
        private async Task<HttpResponseMessage> GetResponsed(HttpClient client, string Action, User data)
        {
            
            var baseuri = string.Format(string.Format("{0}://{1}", _configuration["ServerPath:Protocol"]!, _configuration["ServerPath:Host"]));
            var path = baseuri + Url.Action(Action, EndpointValueInStringStorage.AccountControllerWithWebServer, new {data.Email, data.PasswordHash, data.UserNickName});
            var resp = await client.GetAsync(path);
            return resp;
        }
        
        private async Task<IActionResult> RegisterFromMessageBroker(RegisterViewModel registermodel, (string mainexitaction, string controller) infoForRedirect, string? topic = null)
        {
            var usermap = MappingRegisterModelFromUser(registermodel);
            var cts = new CancellationTokenSource();
            var res = await _messageBroker.Produce(usermap, cts, topic);
            if (!res.Success)
            { 
                _logger.LogError(DateTime.UtcNow + Environment.NewLine + this.ToString() + Environment.NewLine + res.ErrorDescription); 
                return BadRequest();
            }
            var token = JwtCreator.CreateTokenAsync(usermap, _jwtManager);
            var info = new JwtSecurityTokenHandler().ReadJwtToken(token);
            await _signInManager.SignInWithClaimsAsync(usermap, new AuthenticationProperties()
            {
                ExpiresUtc = info.ValidTo
            }, info.Claims);
            return Redirect(Url.Action(infoForRedirect.mainexitaction, infoForRedirect.controller)!);
        }
        private User MappingRegisterModelFromUser(RegisterViewModel model)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<RegisterViewModel, User>()
                  .ForMember("Email", opt => opt.MapFrom(o => o.Mail))
                  .ForMember("PasswordHash", opt => opt.MapFrom(src => src.Password))
                  .ForMember("UserNickName", opt => opt.MapFrom(src => src.Name))
                  .ForMember("LastName", opt => opt.MapFrom(src => src.LastName ?? string.Empty)))); 
            var usermap = mapper.Map<User>(model);
            return usermap;
        }
       
    }
}

