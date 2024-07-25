using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IUsersRolesOperation
    {
        public Task<bool> AddRoleFromUserAsync(string email, string role);
    }
}
