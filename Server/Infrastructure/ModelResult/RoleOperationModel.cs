using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ModelResult
{
    public class RoleOperationModel
    {
        public bool IsSucceced {  get; set; }
        public IList<Exception> errors { get; set; }
    }
}
