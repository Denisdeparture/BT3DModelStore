using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Model.ServiceResultModels
{
    public class ProduceResultModel
    {
        public bool Success { get; internal set; }
        public string? ErrorDescription { get; internal set; }
        public Exception? Exception { get; internal set; }

    }
}
