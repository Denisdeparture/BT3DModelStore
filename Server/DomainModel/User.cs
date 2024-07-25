using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
    public class User : IdentityUser
    {
        public DateTime FirstVisit { get; set; }
        public DateTime LastVisit { get; set; }
        public string? LastName { get; set; }
        public string UserNickName { get; set; } = null!;
        public override string? UserName { get => UserNickName + new Guid().ToString() + Id; set => base.UserName = new Guid().ToString() + Id; }
    }
}
