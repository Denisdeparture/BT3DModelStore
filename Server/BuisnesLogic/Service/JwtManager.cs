using DomainModel;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace WebSever.Service
{
    public class JwtManager
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        public JwtManager(UserManager<User> usermanager, IConfiguration configuration)
        {
            _userManager = usermanager;
            _configuration = configuration;
        }
        public async Task<JwtSecurityToken> CreateJwtTokenForUserAsync(User user)
        {
            var jwt = new JwtSecurityToken(issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            expires:  DateTime.Now.AddMinutes(double.Parse(_configuration["JwtSettings:ExpirationTimeInMinutes"]!)),
            claims: await GetClaims(user),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecurityKey"]!)), SecurityAlgorithms.HmacSha256)
            );;
            return jwt;
        }
        private async Task<List<Claim>> GetClaims(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            foreach(var role in await _userManager.GetRolesAsync(user))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
    }
}
