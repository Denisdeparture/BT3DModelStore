using DataBase.AppDbContexts;
using DomainModel.Entity;
using Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace WebServer.RealizationInterface
{
    public class BucketOperation : IBucketOperation
    {
        private readonly MainDbContext _mainDbContext;
        private readonly UserManager<User> _userManager;
        public BucketOperation(MainDbContext context, UserManager<User> userManager)
        {
            _mainDbContext = context;
            _userManager = userManager;
        }
        public void Add(User user,ProductInBucket product)
        {
            if (product.Quantity <= 0) throw new Exception("This operation is strange because this product has quantity 0! 0_0");
            product.User = user;
            _mainDbContext.AspNetUserProductsInBucket.Add(product);
            _userManager.UpdateAsync(user).Wait();
        }

        public void DecreaseCount(User user, int id)
        {
            var productInBucket = _mainDbContext.AspNetUserProductsInBucket.Where(pr => pr.ProductId == pr.ProductId).FirstOrDefault();
            if (productInBucket is null) throw new NullReferenceException();
            if(productInBucket.Count - 1 <= 0) _mainDbContext.AspNetUserProductsInBucket!.Remove(productInBucket);
            productInBucket.Count--;
            _userManager.UpdateAsync(user).Wait();
        }

        public void IncreaseCount(User user,int id)
        {
            var productInBucket = _mainDbContext.AspNetUserProductsInBucket!.Where(pr => pr.ProductId == pr.ProductId).FirstOrDefault();
            if (productInBucket is null) throw new NullReferenceException();
            if (productInBucket.Count + 1 > productInBucket.Quantity) throw new Exception("This operation unpossible");
            productInBucket.Count++;
            _userManager.UpdateAsync(user).Wait();
        }

        public void Remove(User user,int id)
        {
            var productInBucket = _mainDbContext.AspNetUserProductsInBucket!.Where(pr => pr.ProductId == pr.ProductId).FirstOrDefault();
            if (productInBucket is null) throw new NullReferenceException();
            _mainDbContext.AspNetUserProductsInBucket!.Remove(productInBucket);
            _userManager.UpdateAsync(user).Wait();
        }
    }
}
