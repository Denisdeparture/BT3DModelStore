using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel;
namespace DomainInfrastructure
{
    public interface IUserOperation
    {
        public Task<bool> Register(User user, string? role = null);
        public Task<bool> Update(string id, User newdatauser);
        public IEnumerable<User> GetAllUsers();
    }
}
