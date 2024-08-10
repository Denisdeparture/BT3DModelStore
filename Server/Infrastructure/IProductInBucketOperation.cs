using DomainModel.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IProductInBucketOperation
    {
        public void IncreaseCount(User user,int id);
        public void DecreaseCount(User user, int id);
    }
}
