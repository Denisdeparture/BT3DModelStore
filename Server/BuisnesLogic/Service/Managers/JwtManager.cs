using BuisnesLogic.ConstStorage;
using DomainModel.Entity;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace BuisnesLogic.Service.Managers
{
    public class JwtManager
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        public JwtManager(UserManager<User> usermanager, IConfiguration configuration)
        {
            _userManager = usermanager ?? throw new ArgumentNullException(nameof(usermanager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public JwtSecurityToken CreateJwtTokenForUserAsync(User user)
        {
            var jwt = new JwtSecurityToken(issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JwtSettings:ExpirationTimeInMinutes"]!)),
            claims: GetClaims(user),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecurityKey"]!)), SecurityAlgorithms.HmacSha256)
            ); ;
            return jwt;
        }
        private List<Claim> GetClaims(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(AppClaimsType.NickName, user.UserNickName ?? string.Empty)
            };
            var task = _userManager.GetRolesAsync(user);
            task.Wait();
            var roles = task.Result;
         //   if (roles.Count == 0) throw new NullReferenceException();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
    }
}
