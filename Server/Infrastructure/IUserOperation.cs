using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Entity;
using Infrastructure.ModelResult;
namespace Infrastructure
{
    public interface IUserOperation
    {
        public Task<UserOperationModel> Register(User user);
        public Task<UserOperationModel> UpdateAsync(string id, User newdatauser);
        public IEnumerable<User> GetAllUsers();
    }
}
