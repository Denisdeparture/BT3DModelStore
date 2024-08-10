using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Enum;

namespace DomainModel.Interface
{
    public interface IEntity
    {
        protected int Level { get; set; }
        public virtual PrivacyLevel PrivacyLevel { get => (PrivacyLevel)Level; set => Level = (int)value; }
    }

}
