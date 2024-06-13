using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AppServiceInterfice.Extensions
{
    public static class AuthExtensions
    {
        public static SymmetricSecurityKey GetSymmetricSecurityKey(string sourcekey) => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sourcekey));

    }
}
