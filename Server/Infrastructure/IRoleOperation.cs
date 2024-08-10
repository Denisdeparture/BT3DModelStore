using Infrastructure.ModelResult;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IRoleOperation
    {
        public Task<RoleOperationModel> CreateRole(string name);
        public Task<RoleOperationModel> DeleteRole(string name);
    }
}
