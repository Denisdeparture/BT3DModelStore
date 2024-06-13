using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using WebSever.Service;
namespace WebServer.Controllers
{
    public class AccountEndpointController : ControllerBase
    {
        private readonly JwtManager _jwtManager;
        private readonly UserManager<User> _userManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public AccountEndpointController(JwtManager jwtmanager, UserManager<User> usermanager, RoleManager<IdentityRole> roleManager, ILogger<Program> logger, IConfiguration configuration)
        {
            _jwtManager = jwtmanager;
            _userManager = usermanager;
            _unitOfWork = new UnitOfWork(usermanager,roleManager,logger);
            _configuration = configuration;
        }
        [HttpGet("/AccountEndpoint/Login")]
        public async Task<IActionResult> Login([FromQuery]User obj)
        {
            var user = await _userManager.FindByEmailAsync(obj.Email!);
            if(user == null || !(user.PasswordHash!.Equals( obj.PasswordHash)))
            {
                return Unauthorized();
            }
            return Ok(CreateToken(user));
        }
        [HttpGet("/AccountEndpoint/Registration")]
        public async Task<IActionResult> Registration([FromQuery] User newuser)
        {
            var user = await _userManager.FindByEmailAsync(newuser.Email!);
            if (user != null) return Conflict();
            // ВНИМАНИЕ СТРОКА НИЖЕ ТЕСТОВЫЙ ВАРИАНТ 
            await _unitOfWork.adminOperations.CreateRole("Test");
            var res = await _unitOfWork.userOperations.Register(newuser, _configuration["Roles:User"]!);
            
            var token = await CreateToken(newuser);
            return (res is true) ? Ok(token) : BadRequest();
        }
        private async Task<string> CreateToken(User user)
        {
            var jwt = await _jwtManager.CreateJwtTokenForUserAsync(user);
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return token;
        }
    }
}
