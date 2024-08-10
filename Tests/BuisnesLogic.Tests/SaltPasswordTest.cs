using BuisnesLogic.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Tests
{
    public class SaltPasswordTest
    {
        [Fact]
        public void Test()
        {
            string password = "123456";
            var enc = password.EncryptPassword(28);
            string hashpassword = enc.passwordhash;
            string salt = enc.salt;
            Assert.True(password.PasswordsIsEquals(salt, hashpassword));
        }
    }
}
