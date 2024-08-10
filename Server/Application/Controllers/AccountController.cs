using AutoMapper;
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
using BuisnesLogic.Extensions;
using DomainModel.Entity;
using Newtonsoft.Json.Linq;
using MimeKit;
using BuisnesLogic.Model;
namespace Application.Controllers
{
    public partial class AccountController : Controller
    {
        private readonly ILogger<Program> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IStrategyValidation _validator;
        private readonly IProduceClient<User> _messageBroker;
        private readonly ISender _notification;
        private readonly JwtManager _jwtManager;
        public AccountController(ILogger<Program> logger, SignInManager<User> signInManager, IConfiguration configuration, IStrategyValidation validation, IMessageBrokerClient<User> messageBroker, JwtManager jwtManager, ISender notification)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _validator = validation ?? throw new ArgumentNullException(nameof(validation));
            _messageBroker = messageBroker ?? throw new ArgumentNullException(nameof(messageBroker));
            _jwtManager = jwtManager ?? throw new ArgumentNullException(nameof(jwtManager));
            _notification = notification ?? throw new ArgumentNullException(nameof(notification));
        }
        [HttpGet]
        [ActionName("Login")]
        public async Task<IActionResult> LoginGet()
        {
            return View(new LoginViewModel() { genericDatas = await _signInManager.GetExternalAuthenticationSchemesAsync() });
        }
        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> LoginPost([FromForm] LoginViewModel loginmodel)
        {
            var isvalid = _validator.StrategyCondition(new List<Predicate<LoginViewModel>>() 
            {
                (pw) => !string.IsNullOrEmpty(pw.Mail),
                (pw) => !string.IsNullOrEmpty(pw.Password),
            }, loginmodel);
            if (!isvalid) return View(loginmodel);
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<LoginViewModel, User>()
                  .ForMember("Email", opt => opt.MapFrom(o => o.Mail))
                  .ForMember("PasswordHash", opt => opt.MapFrom(src => src.Password))
                  ));
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
        public IActionResult ProviderAuthentication(string provider)
        {
            const string actionForRedirect = nameof(ProviderAuthenticationCallback);
            var actionUrlFormRedirect = Url.Action(actionForRedirect, EndpointValueInStringStorage.AccountContoller, EndpointValueInStringStorage.MainController);
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
        public IActionResult RegistrationGet([FromQuery] RegisterViewModel registermodel)
        {
            if (registermodel.Mail != null & registermodel.Password != null)
            {
                registermodel.Name = (string.IsNullOrEmpty(registermodel.Name)) ? registermodel.Mail : registermodel.Name;
                var url = Url.Action(EndpointValueInStringStorage.ConfirmEmailAction, EndpointValueInStringStorage.AccountContoller, registermodel);
                return Redirect(url!);
            }
            return View();
        }
        [HttpPost]
        [ActionName("Registration")]
        public IActionResult RegistrationPost([FromForm] RegisterViewModel registermodel)
        {
            if (!ModelState.IsValid)
            {
                return View(registermodel);
            }
            return RedirectToAction(EndpointValueInStringStorage.ConfirmEmailAction, EndpointValueInStringStorage.AccountContoller,  registermodel );
        }
        [HttpGet]
        [ActionName("ConfirmationEmail")]
        public IActionResult ConfirmationEmailGet([FromQuery]RegisterViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Mail)) return BadRequest();
            var provider = model.Mail!.Substring(model.Mail.IndexOf('@') + 1, model.Mail.LastIndexOf('.') - model.Mail.IndexOf('@') - 1);
            string code = string.Empty;
            switch (provider)
            {
                case "yandex":
                    code = SendCodeInMessage(SmtpConfigs.GetConfigs(model.Mail!).YANDEX);
                    break;
                case "gmail":
                    code = SendCodeInMessage(SmtpConfigs.GetConfigs(model.Mail!).GMAIL);
                    break;
                case "mail":
                    code = SendCodeInMessage(SmtpConfigs.GetConfigs(model.Mail!).Mail);
                    break;
                default:
                    return BadRequest();
            }
            return View(new ConfirmViewModel() { Code = code, model = model});
        }
        [HttpPost]
        [ActionName("ConfirmationEmail")]
        public IActionResult ConfirmationEmailPost([FromQuery]RegisterViewModel model)
        {
            if(model is null) return BadRequest();
            var res = Register(model, EndpointValueInStringStorage.CatalogAction);
            res.Wait();
            return res.Result;
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
            
            var baseuri = _configuration.GetServerAddres();
            var path = baseuri + Url.Action(Action, EndpointValueInStringStorage.AccountControllerWithWebServer, new {data.Email, data.PasswordHash, data.UserNickName, data.SaltForPassword});
            var resp = await client.GetAsync(path);
            return resp;
        }
        private async Task<IActionResult> Register(RegisterViewModel registermodel, string mainexitaction)
        {
            var usermap = MappingRegisterModelFromUser(registermodel);
            if (usermap is null) return BadRequest();
            using (var client = new HttpClient())
            {
                var resp = await GetResponsed(client, EndpointValueInStringStorage.RegistrationAction, usermap);
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
                var redirectUrl = Url.Action(mainexitaction, EndpointValueInStringStorage.MainController);
                return Redirect(redirectUrl!);

            }
        }
        private User MappingRegisterModelFromUser(RegisterViewModel model)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<RegisterViewModel, User>()
                  .ForMember("Email", opt => opt.MapFrom(o => o.Mail))
                  .ForMember("PasswordHash", opt => opt.MapFrom(src => src.Password))
                  .ForMember("UserNickName", opt => opt.MapFrom(src => src.Name))
                  .ForMember("LastName", opt => opt.MapFrom(src => src.LastName ?? string.Empty)))); 
            var usermap = mapper.Map<User>(model);
            var passwordInfo = usermap.PasswordHash!.EncryptPassword(28);
            usermap.PasswordHash = passwordInfo.passwordhash;
            usermap.SaltForPassword = passwordInfo.salt;
            return usermap;
        }
        private string SendCodeInMessage(SmtpMessageModel model)
        {
            string code = string.Join("", (Enumerable.Range(0, 5).Select(r => r = new Random().Next(0, 9))).Select(num => num.ToString()));
            var msg = new MimeMessage()
            {
                Subject = "Ваш код подтверждения",
                Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = $"{code}" },
            };
            _notification.SendMailSmtp(msg, model);
            return code;
        }
    }
}

