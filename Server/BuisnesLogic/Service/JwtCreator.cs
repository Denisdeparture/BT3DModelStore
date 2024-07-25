using BuisnesLogic.Service.Managers;
using DomainModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Service
{
    public static class JwtCreator
    {
        public static string CreateTokenAsync(User user, JwtManager manager)
        {
            var jwt =  manager.CreateJwtTokenForUserAsync(user);
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return token;
        }
    }
}
