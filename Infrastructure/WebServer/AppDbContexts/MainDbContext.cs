using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebServer.AppDbContexts
{
    public class MainDbContext : IdentityDbContext<User>
    {
        public MainDbContext(DbContextOptions<MainDbContext> opt) : base(opt)
        {
            //if(!Database.EnsureCreated())
            //{
            //    Database.EnsureCreated();
            //}
            //else
            //{
            //    Database.EnsureDeleted();
            //    Database.EnsureCreated();
            //}
            //if (!Database.EnsureCreated())
            //{
            //    Database.EnsureCreated();
            //}
            Database.EnsureDeleted();
            Database.EnsureCreated();
            
        }
    }
        
}
