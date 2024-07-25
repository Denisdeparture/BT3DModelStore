using Amazon.Runtime;
using BuisnesLogic.Service;
using BuisnesLogic.Service.Managers;
using BuisnesLogic.ServicesInterface;
using Contracts;
using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public AccountEndpointsController(JwtManager jwtmanager, UserManager<User> usermanager, RoleManager<IdentityRole> roleManager, ILogger<Program> logger, IConfiguration configuration, IStrategyValidation validator)
        {
            _jwtManager = jwtmanager;
            _userManager = usermanager;
            _configuration = configuration;
            _logger = logger;
            _validator = validator;
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
            if(userInBase == null || !(userInBase.PasswordHash!.Equals( user.PasswordHash)))
            {
                return Unauthorized();
            }
            var token =  JwtCreator.CreateTokenAsync(userInBase, _jwtManager);

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

    }
}
