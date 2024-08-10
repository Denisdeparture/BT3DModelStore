using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{

    /// <param name="email">Here mail is a search factor</param>
    public record ChangePhoneNumberContract(string email, string phonenumber);
   
}
