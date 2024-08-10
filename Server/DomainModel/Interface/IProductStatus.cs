using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Enum;

namespace DomainModel.Interface
{
    public interface IProductStatus
    {
        protected int Status { get; set; }
        public virtual ProductStatus ProductStatus { get => (ProductStatus)Status; set => Status = (int)value; }
        protected int PayStatus { get; set; }
        public virtual PayStatus ProductPayStatus { get => (PayStatus)PayStatus; set => PayStatus = (int)value; }
    }
}
