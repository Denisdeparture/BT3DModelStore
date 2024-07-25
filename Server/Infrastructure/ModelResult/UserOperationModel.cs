using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ModelResult
{
    public class UserOperationModel
    {
        public bool IsSuccesed { get; set; }
        public IList<Exception>? Errors {  get; set; } 

        
    }
}
