using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BuisnesLogic.Model.Roles;
namespace DataBase.AppDbContexts
{
    public class MainDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public DbSet<Product> Products => Set<Product>();
        public MainDbContext(DbContextOptions<MainDbContext> opt) : base(opt)
        {
            if (!Database.EnsureCreated())
            {
                Database.EnsureCreated();
            }
        }

    }
        
}
