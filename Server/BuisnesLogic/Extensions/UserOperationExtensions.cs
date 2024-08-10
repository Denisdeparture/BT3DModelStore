using Infrastructure;
using Infrastructure.ModelResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Extensions
{
    public static class UserOperationExtensions
    {
        public static async Task<UserOperationModel> UpdateViaMailAsync(this IUserOperation userOperation, string oldemail, string newemail)
        {
            var user = userOperation.GetAllUsers().Where(u => u.Email == oldemail).SingleOrDefault();
            var anotherUserHasThisEmail = userOperation.GetAllUsers().Where(u => u.Email == newemail).SingleOrDefault();
            if (anotherUserHasThisEmail is not null) throw new Exception("Another user just had this email");
            if (user is null) throw new NullReferenceException(nameof(user));
            user.Email = newemail;
            var res = await userOperation.UpdateAsync(user.Id, user);
            return res;
        }
        public static async Task<UserOperationModel> UpdateViaTelephoneFromEmailAsync(this IUserOperation userOperation,string email, string telephone)
        {
            var user = userOperation.GetAllUsers().Where(u => u.Email == email).SingleOrDefault();
            if (user is null) throw new NullReferenceException(nameof(user));
            user.PhoneNumber = telephone;
            var res = await userOperation.UpdateAsync(user.Id, user);
            return res;
        }
    }
}
