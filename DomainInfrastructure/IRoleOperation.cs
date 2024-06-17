using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainInfrastructure
{
    public interface IRoleOperation
    {
        public Task<bool> CreateRole(string name);
        public Task<bool> DeleteRole(string name);
    }
}
