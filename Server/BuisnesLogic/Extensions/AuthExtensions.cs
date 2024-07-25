using DomainModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BuisnesLogic.Extensions
{
    public static class AuthExtensions
    {
        public static SymmetricSecurityKey GetSymmetricSecurityKey(string sourcekey) => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sourcekey));
        public async static Task<T?> FindByNameAsync<T>(this UserManager<T> userManager, string name, string email) where T : User
        {
            var res = await userManager.FindByEmailAsync(email);
            if (res is null) return null;
            return res.UserNickName.Equals(name) ? res : null;
        }
    }
}
