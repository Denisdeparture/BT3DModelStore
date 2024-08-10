using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BuisnesLogic.Model.Roles;
using DomainModel.Entity;
using System.Reflection.Metadata;
namespace DataBase.AppDbContexts
{
    public class MainDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductInBucket> AspNetUserProductsInBucket => Set<ProductInBucket>(); 
        public DbSet<PurchasedProduct> AspNetUserPurchasedProducts => Set<PurchasedProduct>();
        public MainDbContext(DbContextOptions<MainDbContext> opt) : base(opt)
        //{

        //}
        {
            if (!Database.EnsureCreated())
            {
                Database.EnsureCreated();
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                     .HasMany(e => e.ProductsInBucket)
                     .WithOne(e => e.User)
                     .HasForeignKey(e => e.ProductId)
                     .IsRequired();
                  
        }
    }
        
}
