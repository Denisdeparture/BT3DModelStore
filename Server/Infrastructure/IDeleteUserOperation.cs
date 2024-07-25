using Infrastructure.ModelResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IDeleteUserOperation
    {
        public Task<UserOperationModel> Delete(string id);
    }
}
