using Infrastructure.ModelResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IUsersRolesOperation
    {
        public Task<RoleOperationModel> AddRoleFromUserAsync(string email, string role);
    }
}
