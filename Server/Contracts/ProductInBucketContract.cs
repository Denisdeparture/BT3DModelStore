using DomainModel.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class ProductInBucketContract
    {
        public string UserEmail { get; set; } = null!;
        public ProductInBucket Product { get; set; } = null!;
    }
}
