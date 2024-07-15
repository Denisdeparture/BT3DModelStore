using BuisnesLogic.Service;
using BuisnesLogic.Service.Managers;
using Contracts;
using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Text.Json;
namespace WebServer.Controllers
{
    public class AccountEndpointsController : ControllerBase
    {
        private readonly JwtManager _jwtManager;
        private readonly UserManager<User> _userManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Program> _logger;
        public AccountEndpointsController(JwtManager jwtmanager, UserManager<User> usermanager, RoleManager<IdentityRole> roleManager, ILogger<Program> logger, IConfiguration configuration)
        {
            _jwtManager = jwtmanager;
            _userManager = usermanager;
            _unitOfWork = new UnitOfWork(usermanager,roleManager,logger);
            _configuration = configuration;
            _logger = logger;
        }
        [HttpGet("/AccountEndpoint/Login")]
        public async Task<IActionResult> Login([FromQuery]User obj)
        {
           
            var user = await _userManager.FindByEmailAsync(obj.Email!);
            if(user == null || !(user.PasswordHash!.Equals( obj.PasswordHash)))
            {
                return Unauthorized();
            }
            var token = await JwtCreator.CreateTokenAsync(user, _jwtManager);

            return Ok(JsonSerializer.Serialize(new UserJwtContract() { JwtToken = token, User = user}));
        }
        

    }
}
