using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class UserJwtContract 
    {
        public string JwtToken { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
