using BuisnesLogic.Service.Safety;
using DomainModel.Entity;
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
        public static (string passwordhash, string salt) EncryptPassword(this string password, uint amount_of_salt)
        {
            SaltPassword salt = new SaltPassword();
            var enc = salt.Salt(password, amount_of_salt);
            return new(enc.hash, enc.salt);
        }
        public static bool PasswordsIsEquals(this string password, string salt, string passwordHash)
        {
            SaltPassword saltpass = new SaltPassword();
            return saltpass.Compare(password, salt, passwordHash);
        }
    }
}
