using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebClient.DbContexts
{
    public class SignInDbContext : IdentityDbContext
    {
        public SignInDbContext(DbContextOptions opt) : base(opt)
        {
            if (!Database.EnsureCreated())
            {
                Database.EnsureCreated();
            }
        }
    }
}
