using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainInfrastructure
{
    public interface IDeleteUserOperation
    {
        public Task<bool> Delete(string id);
    }
}
