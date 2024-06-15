using DomainModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebClient.DbContexts
{
    public class MainClientDbContext : IdentityDbContext
    {
        public MainClientDbContext(DbContextOptions opt) : base(opt)
        {
            if (!Database.EnsureCreated())
            {
                Database.EnsureCreated();
            }
        }
    }
}
