using BuisnesLogic.Model.ServiceResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ServicesInterface.ClientsInterfaces.MessageBrokers
{
    public interface IProduceClient<TModel>
    {
        public Task<ProduceResultModel> Produce(TModel model, CancellationTokenSource cts, string? topic = null);
    }
}
