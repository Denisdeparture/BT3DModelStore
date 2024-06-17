using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebServer.AppDbContexts
{
    public class MainDbContext : IdentityDbContext<User>
    {
        public DbSet<Product> Products => Set<Product>();
        public MainDbContext(DbContextOptions<MainDbContext> opt) : base(opt)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
            
        }
    }
        
}
