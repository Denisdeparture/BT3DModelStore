using BuisnesLogic.ServicesInterface;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Service.Safety
{
    public class SaltPassword 
    {
       
        public bool Compare(string password, string salt, string hashpassword)
        {
            if (string.IsNullOrWhiteSpace(password) | string.IsNullOrWhiteSpace(salt)) throw new ArgumentNullException();
            var hash = GetSaltedHashedPassword(password, salt);
            return hash.Equals(hashpassword);
        }
        public (string hash, string salt) Salt(string password, uint length)
        {
            if(string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));
            using (var generator = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[(int)length];
                generator.GetBytes(bytes);
                string salt = Convert.ToBase64String(bytes);
                string saltedhashedPassword = GetSaltedHashedPassword(password, salt);
                return (saltedhashedPassword, salt);
            }
        }
        private string GetSaltedHashedPassword(string password, string salt)
        {
            string saltedPassword = String.Concat(password, salt);
            byte[] saltedPasswordBytes = Encoding.Unicode.GetBytes(saltedPassword);
            SHA256 sha = SHA256.Create();
            byte[] saltedPasswordHashBytes = sha.ComputeHash(saltedPasswordBytes);
            string result = Convert.ToBase64String(saltedPasswordHashBytes);
            return result;
        }
    }
}
