using DomainModel.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IBucketOperation : IProductInBucketOperation
    {
        public void Add(User user, ProductInBucket product);
        public void Remove(User user,int id);
    }
}
