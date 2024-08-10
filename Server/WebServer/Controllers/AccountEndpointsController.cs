using Amazon.Runtime;
using BuisnesLogic.ConstStorage;
using BuisnesLogic.Extensions;
using BuisnesLogic.Service;
using BuisnesLogic.Service.Managers;
using BuisnesLogic.ServicesInterface;
using BuisnesLogic.ServicesInterface.ClientsInterfaces.MessageBrokers;
using Contracts;
using DomainModel.Entity;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RazorLight.Extensions;
using System.Text.Json;
namespace WebServer.Controllers
{
    public class AccountEndpointsController : ControllerBase
    {
        private readonly JwtManager _jwtManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Program> _logger;
        private readonly IStrategyValidation _validator;
        private readonly IUserOperation _userOperation;
        public AccountEndpointsController(JwtManager jwtmanager, UserManager<User> usermanager,IUserOperation userOperation,RoleManager<IdentityRole> roleManager, ILogger<Program> logger, IConfiguration configuration, IStrategyValidation validator)
        {
            _jwtManager = jwtmanager;
            _userManager = usermanager;
            _configuration = configuration;
            _logger = logger;
            _validator = validator;
           _userOperation = userOperation;
        }
        [HttpGet("/AccountEndpoints/Login")]
        public async Task<IActionResult> Login([FromQuery]User user)
        {
            
            var isValid = _validator.StrategyCondition(new List<Predicate<User>>()
            {
                (pw) => pw != null
            }, user);
            if (!isValid) return BadRequest();
            var userInBase = await _userManager.FindByEmailAsync(user.Email!);
            if (userInBase == null || !(user.PasswordHash!.PasswordsIsEquals(userInBase.SaltForPassword, userInBase.PasswordHash!)))
            {
                return Unauthorized();
            }
            var token =  JwtCreator.CreateToken(userInBase, _jwtManager);

            return Ok(JsonSerializer.Serialize(new UserJwtContract() { JwtToken = token, User = userInBase}));
        }
        [HttpGet("/AccountEndpoints/GetUserInfo")]
        public async Task<IActionResult> GetUserInfo([FromQuery] string email)
        {
            var isValid = _validator.StrategyCondition(new List<Predicate<string>>() {
                (pw) => !string.IsNullOrWhiteSpace(pw),
            }, email);
            if(!isValid) return BadRequest();
            var res = await _userManager.FindByEmailAsync(email!);
            return res is not null ? Ok(JsonSerializer.Serialize<User>(res)) : NotFound();

        }
        [HttpGet("/AccountEndpoints/Registration")]
        public async Task<IActionResult> Registration([FromQuery] User newuser)
        {
            string token = string.Empty;
            var user = await _userManager.FindByEmailAsync(newuser.Email!);
            if (user != null)
            {
                token = JwtCreator.CreateToken(user, _jwtManager);
                return Ok(token);
            }
            var cts = new CancellationTokenSource();
            var res = await _userOperation.Register(newuser);
            token = JwtCreator.CreateToken(newuser, _jwtManager);
            return res.IsSuccesed ? Ok(token) : BadRequest();
        }
        [HttpPut("/AccountEndpoints/ChangePhoneNumber")]
        public async Task<IActionResult> ChangePhoneNumber()
        {
            try
            {
                var model = await HttpContext.Request.ReadFromJsonAsync<ChangePhoneNumberContract>();
                if (model is null || string.IsNullOrWhiteSpace(model.phonenumber) || string.IsNullOrWhiteSpace(model.email)) return BadRequest();
                var res = await _userOperation.UpdateViaTelephoneFromEmailAsync(model.email, model.phonenumber);
                if (!res.IsSuccesed)
                {
                    if (res.Errors is not null && res.Errors.Count != 0) _logger.LogError(string.Join(Environment.NewLine, res.Errors.Select(ex => ex.Message)));
                    return BadRequest();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }
        [HttpPut("/AccountEndpoints/ChangeEmail")]
        public async Task<IActionResult> ChangeEmail()
        {
            try
            {
                var model = await HttpContext.Request.ReadFromJsonAsync<ChangeEmailContract>();
                if (model is null || string.IsNullOrWhiteSpace(model.oldemail) || string.IsNullOrWhiteSpace(model.newemail)) return BadRequest(); 
                var res = await _userOperation.UpdateViaMailAsync(model.oldemail, model.newemail);
                if (!res.IsSuccesed)
                {
                    if(res.Errors is not null && res.Errors.Count != 0) _logger.LogError(string.Join(Environment.NewLine,res.Errors.Select(ex => ex.Message)));
                    return BadRequest();
                }
                var user = _userOperation.GetAllUsers().Where(u => u.Email == model.newemail).SingleOrDefault();
               
                return user is not null ? Ok(new UserJwtContract() { JwtToken = JwtCreator.CreateToken(user, _jwtManager), User = user }) : BadRequest();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
            

        }
    }
}
